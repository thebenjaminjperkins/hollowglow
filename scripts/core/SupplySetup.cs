using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Hollowlight.Core;

public sealed record GatheringSite(string Id, string Name, string ResourceId, int Quantity, int X, int Y, string Asset);
public sealed record SupplySetup(ResourceDefinition[] Resources, GatheringSite[] Sites)
{
    public static SupplySetup Parse(string json) => JsonSerializer.Deserialize<SupplySetup>(json,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new JsonException("Missing supply setup.");
    public SharedSupplies CreateState(IEnumerable<int> playerIds, int storageSlots, IEnumerable<ResourceDefinition>? extra = null) => new(Resources.Concat(extra ?? System.Array.Empty<ResourceDefinition>()),
        Sites.Select(s => new ResourceNodeDefinition(s.Id, s.ResourceId, s.Quantity)), playerIds, storageSlots);
}
