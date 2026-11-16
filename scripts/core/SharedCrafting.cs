using System;
using System.Collections.Generic;
using System.Linq;

namespace Hollowlight.Core;

public sealed partial class SharedSupplies
{
    public const string ExpansionPlotId="garden-expansion";
    private CraftingSetup? _crafting;
    private RecipeDefinition[] _recipes=Array.Empty<RecipeDefinition>();
    public void InitializeCrafting(CraftingSetup setup)
    {
        lock(_gate)
        {
            if(_crafting!=null)throw new InvalidOperationException("Crafting initializes once per session.");
            if(_power==null || _agriculture==null)throw new InvalidOperationException("Crafting needs session power and agriculture.");
            var recipes=setup.ValidatedRecipes(_resources);
            if(recipes.Any(r=>!_consumers.Any(c=>c.Id==r.Consumer)))throw new ArgumentException("Recipe requires an unknown consumer.");
            if(Farm.Plots.Count>=setup.MaximumPlots)throw new ArgumentException("Crafting needs one expansion position.");
            _crafting=setup;_recipes=recipes;
        }
    }
    private CraftingSetup Crafting=>_crafting??throw new InvalidOperationException("Crafting is not initialized.");
    private int KitCount(string id)=>_storage.Count(id)+_players.Values.Sum(p=>p.Count(id));
    private bool BedInstalled=>Farm.Plots.ContainsKey(ExpansionPlotId);
    private CraftStatus CraftAvailability(Inventory player,RecipeDefinition recipe)
    {
        if(RunCompleted)return CraftStatus.AccessDenied;
        if(recipe.Outputs.ContainsKey(Crafting.BedKit) && (BedInstalled||Farm.Plots.Count>=Crafting.MaximumPlots||KitCount(Crafting.BedKit)>0))return CraftStatus.LimitReached;
        if(recipe.Outputs.ContainsKey(Crafting.BackpackKit) &&
            _players.Values.Count(p=>p.Capacity<Crafting.BackpackSlots)<=KitCount(Crafting.BackpackKit))return CraftStatus.LimitReached;
        if(!_running||!_enabledConsumers.Contains(recipe.Consumer))return CraftStatus.NoPower;
        if(recipe.Inputs.Any(p=>player.Count(p.Key)<p.Value))return CraftStatus.Insufficient;
        var trial=player.Copy();
        foreach(var input in recipe.Inputs)trial.Remove(input.Key,input.Value);
        return trial.CanAddAll(recipe.Outputs.Select(p=>new ItemStack(p.Key,p.Value)))?CraftStatus.Applied:CraftStatus.NoCapacity;
    }
    public CraftingSnapshot CraftingFor(int playerId)
    {
        lock(_gate)
        {
            _=Crafting;
            if(!_players.TryGetValue(playerId,out var player))throw new ArgumentOutOfRangeException(nameof(playerId));
            return new(Version,PowerVersion,player.Capacity>PlayerSlots,BedInstalled,
                Array.AsReadOnly(_recipes.Select(r=>new RecipeAvailability(r,CraftAvailability(player,r))).ToArray()));
        }
    }
    public CraftResult Craft(int playerId,CraftRequest request)
    {
        lock(_gate)
        {
            _=Crafting;
            CraftResult Result(CraftStatus status)=>new(status,Version);
            if(RunCompleted)return Result(CraftStatus.AccessDenied);
            if(!_players.TryGetValue(playerId,out var player))return Result(CraftStatus.InvalidPlayer);
            if(request.Version!=Version || request.PowerVersion!=PowerVersion)return Result(CraftStatus.Stale);
            var recipe=_recipes.FirstOrDefault(r=>r.Id==request.RecipeId);
            if(recipe==null)return Result(CraftStatus.UnknownRecipe);
            var available=CraftAvailability(player,recipe);
            if(available!=CraftStatus.Applied)return Result(available);
            // Preflight and commit share the session lock; no material leaves during UI previews.
            var trial=player.Copy();
            foreach(var input in recipe.Inputs)trial.Remove(input.Key,input.Value);
            foreach(var output in recipe.Outputs)trial.Add(output.Key,output.Value);
            _players[playerId]=trial;_revision++;
            return Result(CraftStatus.Applied);
        }
    }
    public CraftResult InstallBackpack(int playerId,InstallRequest request)=>Install(playerId,request,false);
    public CraftResult InstallBed(int playerId,InstallRequest request)=>Install(playerId,request,true);
    private CraftResult Install(int playerId,InstallRequest request,bool bed)
    {
        lock(_gate)
        {
            _=Crafting;
            CraftResult Result(CraftStatus status)=>new(status,Version);
            if(RunCompleted)return Result(CraftStatus.AccessDenied);
            if(!_players.TryGetValue(playerId,out var player))return Result(CraftStatus.InvalidPlayer);
            if(request.Version!=Version)return Result(CraftStatus.Stale);
            if(!player.Valid(request.Slot))return Result(CraftStatus.InvalidSlot);
            if(player.At(request.Slot)?.ResourceId!=(bed?Crafting.BedKit:Crafting.BackpackKit))return Result(CraftStatus.WrongItem);
            if(bed ? BedInstalled||Farm.Plots.Count>=Crafting.MaximumPlots : player.Capacity>=Crafting.BackpackSlots)
                return Result(CraftStatus.AlreadyInstalled);
            var trial=player.Copy(bed?player.Capacity:Crafting.BackpackSlots);trial.Take(request.Slot,1);
            if(bed)Farm.Plots.Add(ExpansionPlotId,new Agriculture.Plot());
            _players[playerId]=trial;_revision++;
            return Result(CraftStatus.Applied);
        }
    }
}
