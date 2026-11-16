using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;

namespace Hollowlight.Core;

public sealed record RecipeDefinition(string Id, string Name, string Icon, string Consumer, string Description,
    IReadOnlyDictionary<string,int> Inputs, IReadOnlyDictionary<string,int> Outputs);
public sealed record CraftingSetup(ResourceDefinition[] Resources, RecipeDefinition[] Recipes, string BackpackKit,
    string BedKit, int BackpackSlots, int MaximumPlots, PlotOffset SocketOffset)
{
    public static CraftingSetup Parse(string json) => JsonSerializer.Deserialize<CraftingSetup>(json,
        new JsonSerializerOptions { PropertyNameCaseInsensitive=true }) ?? throw new JsonException("Missing crafting setup.");
    internal RecipeDefinition[] ValidatedRecipes(IReadOnlyDictionary<string,ResourceDefinition> resources)
    {
        if(BackpackSlots<=SharedSupplies.PlayerSlots || BackpackSlots>8 || MaximumPlots!=5 || BackpackKit==BedKit ||
            !resources.ContainsKey(BackpackKit) || !resources.ContainsKey(BedKit) || Recipes.Length==0 || Recipes.Length>16 ||
            Recipes.Select(r=>r.Id).Distinct().Count()!=Recipes.Length || !float.IsFinite(SocketOffset.X) || !float.IsFinite(SocketOffset.Y))
            throw new ArgumentException("Invalid crafting setup.");
        return Recipes.Select(r=> {
            if(string.IsNullOrWhiteSpace(r.Id)||string.IsNullOrWhiteSpace(r.Name)||string.IsNullOrWhiteSpace(r.Consumer)||
                string.IsNullOrWhiteSpace(r.Description)||!resources.ContainsKey(r.Icon)||r.Inputs.Count==0||r.Outputs.Count==0||
                r.Inputs.Concat(r.Outputs).Any(p=>!resources.ContainsKey(p.Key)||p.Value<1||p.Value>2560)||
                r.Inputs.ContainsKey(BackpackKit)||r.Inputs.ContainsKey(BedKit)||
                r.Outputs.TryGetValue(BackpackKit,out int bags)&&bags!=1||r.Outputs.TryGetValue(BedKit,out int beds)&&beds!=1)
                throw new ArgumentException("Invalid recipe.");
            return r with {Inputs=new ReadOnlyDictionary<string,int>(r.Inputs.ToDictionary(p=>p.Key,p=>p.Value)),
                Outputs=new ReadOnlyDictionary<string,int>(r.Outputs.ToDictionary(p=>p.Key,p=>p.Value))};
        }).ToArray();
    }
}
public enum CraftStatus { Applied, Stale, InvalidPlayer, UnknownRecipe, Insufficient, NoCapacity, NoPower,
    InvalidSlot, WrongItem, AlreadyInstalled, LimitReached, AccessDenied }
public sealed record CraftRequest(string RecipeId, SupplyVersion Version, SupplyVersion PowerVersion);
public sealed record InstallRequest(int Slot, SupplyVersion Version);
public sealed record CraftResult(CraftStatus Status, SupplyVersion Version);
public sealed record RecipeAvailability(RecipeDefinition Recipe, CraftStatus Status);
public sealed record CraftingSnapshot(SupplyVersion Version, SupplyVersion PowerVersion, bool BackpackInstalled,
    bool BedInstalled, IReadOnlyList<RecipeAvailability> Recipes);
