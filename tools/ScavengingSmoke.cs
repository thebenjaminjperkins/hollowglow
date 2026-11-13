using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using Hollowlight.Core;
using Hollowlight.Presentation;
using Hollowlight.World;

namespace Hollowlight.Tools;

public partial class ScavengingSmoke : Node
{
    private static bool _reset;
    private static string _initial = "";
    private ThresholdWorld _world = null!;
    private PlayerCharacter _actor = null!;
    private SuppliesSnapshot Stock => _world.SuppliesFor(1);
    private IReadOnlyList<GeneratedSourceSite> Sources => _world.ScavengingWorld!.Sources;
    public override async void _Ready()
    {
        try
        {
            var room = GD.Load<PackedScene>(ProjectSettings.GetSetting("application/run/main_scene").AsString()).Instantiate<SanctuaryRoom>();
            AddChild(room); _world = room.World; _actor = _world.GetNode<PlayerCharacter>("Player"); await Frames(3);
            string generated = JsonSerializer.Serialize(_world.ScavengingWorld);
            Check(Sources.Count == 6 && Stock.Personal.Count("food") == 2 && _world.GrowingPlots.Count == 4,
                "Normal refuge has six generated sources, unchanged personal starters and four farm plots");
            if (_reset)
            {
                Check(generated == _initial && Sources.All(s => Stock.Remaining[s.Site.Id] == s.Site.Quantity) && _world.HungerFor(1)!.Consumed == 0,
                    "Reset restores identical seeded placements/quantities, fresh depletion and hunger");
                GD.Print("PASS: live seeded Food scavenging, reachability, E gathering, partial/full capacity, transfer/eat, depletion/no respawn and reset"); GetTree().Quit(); return;
            }
            _initial = generated;
            foreach (string type in new[] { "container", "wild_plant" })
            {
                var source = Sources.First(s => s.Type == type); await Approach(source); await Capture("available-" + type, source);
            }
            foreach (var source in Sources)
            {
                _actor.Position = new Vector2(source.ApproachX, source.ApproachY); await Frames(2);
                var query = new PhysicsShapeQueryParameters2D { Shape = new RectangleShape2D { Size = new Vector2(8, 6) },
                    Transform = new Transform2D(0, _actor.Position), CollisionMask = 1 };
                Check(_world.GetWorld2D().DirectSpaceState.IntersectShape(query).Count == 0, $"Clear live feet collision at {source.Site.Id} approach");
                Check(Art(source).StateFrame == 0 && Target(source).CollisionLayer == 0, "Available PNG state and unchanged traversable ground");
            }
            _actor.Position = new Vector2(136, 208); await Frames(2);
            Check(_world.GatherSupplies(_actor, Sources[0].Site.Id, Stock.Version).Status == SupplyStatus.AccessDenied, "Out-of-range Food request rejected");
            if (_world.ScavengingWorld!.Seed == 1701)
            {
                // Fill five non-Food slots using original finite salvage and one split.
                foreach (var site in SupplySetup.Parse(FileAccess.GetFileAsString("res://data/supplies.json")).Sites)
                {
                    _actor.Position = new Vector2(site.X, site.Y + 18); await Frames(2);
                    Check(_world.GatherSupplies(_actor, site.Id, Stock.Version).Status == SupplyStatus.Applied, "Existing finite salvage remains gatherable");
                }
                SendKey(Godot.Key.Tab); await Frames(2);
                int scrap = Stock.Personal.Slots.ToList().FindIndex(x => x?.ResourceId == "scrap");
                Check(_world.ApplyInventoryCommand(_actor, new(SlotAction.Move, new(InventoryKind.Personal, scrap), new(InventoryKind.Personal, 5), 1, Stock.Version)).Status == SupplyStatus.Applied,
                    "Existing split operation fills final slot without losing items");
                _world.CloseInventory();
                int subset = -1, partialIndex = -1;
                for (int mask = 0; mask < 64 && subset < 0; mask++)
                {
                    int sum = Enumerable.Range(0, 6).Where(i => (mask & (1 << i)) != 0).Sum(i => Sources[i].Site.Quantity);
                    int remaining = Enumerable.Range(0, 6).FirstOrDefault(i => (mask & (1 << i)) == 0 && Sources[i].Site.Quantity >= 2, -1);
                    if (sum == 7 && remaining >= 0) { subset = mask; partialIndex = remaining; }
                }
                Check(subset >= 0, "Default seeded world supports capacity-edge playthrough");
                for (int i = 0; i < 6; i++) if ((subset & (1 << i)) != 0) await Gather(Sources[i]);
                Check(Stock.Personal.Count("food") == 9 && Stock.Personal.UsedSlots == 6, "Live pack has nine Food and six occupied slots");
                var partial = Sources[partialIndex]; await Approach(partial); await Capture("available", partial);
                int before = Stock.Remaining[partial.Site.Id]; await Gather(partial);
                Check(Stock.Personal.Count("food") == 10 && Stock.Remaining[partial.Site.Id] == before - 1 && Art(partial).StateFrame == 0,
                    "Partial gather fills existing stack and leaves source visibly available");
                var unchanged = Stock; await Gather(partial);
                Check(Stock.Version == unchanged.Version && Stock.Remaining[partial.Site.Id] == before - 1, "Full pack preserves source and inventory");
                await Chest(); _world.InventoryPanel.StoreAll(); await Frames(2); _world.CloseInventory();
                await Gather(partial);
                Check(Target(partial).Visible && Target(partial).Depleted && Art(partial).StateFrame == 1, "Returning after deposit collects remainder and shows depleted PNG");
            }
            // Gather every source's remainder through the normal contextual interaction.
            foreach (var source in Sources) if (Stock.Remaining[source.Site.Id] > 0) await Gather(source);
            Check(Sources.All(s => Stock.Remaining[s.Site.Id] == 0), "All six sources exhaust completely");
            foreach (string type in new[] { "container", "wild_plant" })
            {
                var source = Sources.First(s => s.Type == type); await Approach(source); await Capture("depleted-" + type, source);
                Check(Target(source).Visible && Art(source).StateFrame == 1, "Both depleted source types remain visible");
                var version = Stock.Version; await InteractKey();
                Check(Stock.Version == version && Stock.Remaining[source.Site.Id] == 0, "Repeated interaction with depleted source creates no Food");
            }
            await Chest();
            _world.InventoryPanel.StoreAll(); await Frames(2);
            int total = Sources.Sum(s => s.Site.Quantity) + 2;
            Check(Stock.Storage.Count("food") == total, "All scavenged Food plus unchanged starters conserved through chest deposit");
            Check(_world.TransferSupplies(_actor, "food", 1, true, Stock.Version)?.Amount == 1, "Scavenged Food can be withdrawn normally");
            int foodSlot = Stock.Personal.Slots.ToList().FindIndex(x => x?.ResourceId == "food");
            Check(_world.EatFood(_actor, new(foodSlot, Stock.Version, _world.HungerFor(1)!.Day)).Status == EatStatus.Eaten,
                "Existing meal system accepts scavenged Food");
            _world.CloseInventory(); _world.Clock.Advance(1800); await Frames(3);
            Check(Sources.All(s => Stock.Remaining[s.Site.Id] == 0) && Sources.All(s => Target(s).Visible && Art(s).StateFrame == 1),
                "Time skips and leaving sources never respawn Food");
            _reset = true; SendKey(Godot.Key.R);
        }
        catch (Exception error) { GD.PushError(error.ToString()); GetTree().Quit(1); }
    }
    private Interactable Target(GeneratedSourceSite source) => _world.GetChildren().OfType<Interactable>().Single(t => t.ObjectId == source.Site.Id);
    private ScavengingArt Art(GeneratedSourceSite source) => Target(source).GetNode<ScavengingArt>("FoodArt");
    private async Task Approach(GeneratedSourceSite source) { _actor.Position = new(source.ApproachX, source.ApproachY); await Frames(3); }
    private async Task Gather(GeneratedSourceSite source)
    {
        await Approach(source); await InteractKey();
    }
    private async Task Chest() { _actor.Position = new Vector2(88, 242); await Frames(3); await InteractKey(); }
    private void SendKey(Godot.Key key) => _world._UnhandledKeyInput(new InputEventKey { PhysicalKeycode = key, Pressed = true });
    private async Task InteractKey()
    {
        Input.ParseInputEvent(new InputEventKey { PhysicalKeycode = Godot.Key.E, Pressed = true }); await Frames(2);
        Input.ParseInputEvent(new InputEventKey { PhysicalKeycode = Godot.Key.E, Pressed = false }); await Frames(2);
    }
    private async Task Frames(int count)
    { for (int i = 0; i < count; i++) { await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame); await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame); } }
    private async Task Capture(string name, GeneratedSourceSite source)
    {
        if (!OS.GetCmdlineUserArgs().Contains("--capture-scavenging")) return;
        Vector2 saved = _actor.Position;
        _actor.Position = new Vector2(source.Site.X - 12, source.Site.Y + 4);
        await Frames(6); await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);
        string folder = ProjectSettings.GlobalizePath("user://scavenging"); DirAccess.MakeDirRecursiveAbsolute(folder);
        Check(GetViewport().GetTexture().GetImage().SavePng($"{folder}/{name}.png") == Error.Ok, "Capture " + name);
        _actor.Position = saved; await Frames(3);
    }
    private static void Check(bool value, string message)
    { if (!value) throw new InvalidOperationException(message); GD.Print("PASS: " + message); }
}
