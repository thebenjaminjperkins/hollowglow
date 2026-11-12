using System;
using System.Text.Json;

namespace Hollowlight.Core;

public sealed record HungerDefinition(int DailyFood = 1, int StartingFood = 2, string StartingResource = "food",
    int RecoveryPerMeal = 1, int StarvingAt = 2, int CriticalAt = 3,
    double StarvingSpeed = 0.9, double CriticalSpeed = 0.8)
{
    public static HungerDefinition Parse(string json) => JsonSerializer.Deserialize<HungerDefinition>(json,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new JsonException("Missing hunger definition.");
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(StartingResource) || DailyFood < 1 || DailyFood > SharedSupplies.PlayerSlots * SharedSupplies.StackLimit || StartingFood < 0 ||
            StartingFood > SharedSupplies.PlayerSlots * SharedSupplies.StackLimit || RecoveryPerMeal < 1 ||
            StarvingAt < 2 || CriticalAt <= StarvingAt || !double.IsFinite(StarvingSpeed) || !double.IsFinite(CriticalSpeed) ||
            CriticalSpeed <= 0 || StarvingSpeed > 1 || CriticalSpeed > StarvingSpeed)
            throw new ArgumentException("Invalid hunger configuration.");
    }
}
public enum HungerStage { Fed, Hungry, Starving, Critical }
public sealed record HungerSnapshot(long Day, int Eaten, int Required, int MissedMeals, HungerStage Stage,
    double SpeedMultiplier, long NextMealTicks, long Consumed, long MissedDeadlines, long SatisfiedDays);
public sealed record EatRequest(int Slot, SupplyVersion Version, long Day);
public enum EatStatus { Eaten, Satisfied, Stale, InvalidPlayer, InvalidSlot, NotEdible, Empty, AccessDenied }
public sealed record EatResult(EatStatus Status, int Amount);

internal sealed class MealState
{
    public int Eaten, Missed;
    public long Consumed, MissedDeadlines, SatisfiedDays;
}
