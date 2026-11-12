using System;
using System.Collections.Generic;
using System.Linq;

namespace Hollowlight.Core;

public sealed partial class SharedSupplies
{
    private HungerDefinition? _hunger;
    private GameClock? _mealClock;
    private long _mealDay;
    private readonly Dictionary<int, MealState> _meals = new();

    // Initialize once, before exposing the session. Starter allocation is all-or-nothing.
    public void InitializeHunger(HungerDefinition definition, GameClock clock)
    {
        lock (_gate)
        {
            if (_hunger != null) throw new InvalidOperationException("Hunger is initialized once per session.");
            definition.Validate();
            if (_agriculture != null && !ReferenceEquals(Farm.Clock, clock))
                throw new ArgumentException("Farming and meals must share the session clock.");
            if (!_resources.TryGetValue(definition.StartingResource, out var food) || !food.Edible)
                throw new ArgumentException("Starting Food must be an edible resource.");
            if (_players.Values.Any(p => p.RoomFor(food.Id) < definition.StartingFood))
                throw new ArgumentException("Starting Food must fit every personal inventory.");
            clock.BindAuthority(_gate, SettleMeals);
            _hunger = definition; _mealClock = clock; _mealDay = clock.Snapshot().DayNumber;
            foreach (var pair in _players)
            {
                pair.Value.Add(food.Id, definition.StartingFood);
                _meals.Add(pair.Key, new());
            }
            if (definition.StartingFood > 0) _revision++;
        }
    }
    public HungerSnapshot Hunger(int playerId)
    {
        lock (_gate)
        {
            var def = _hunger ?? throw new InvalidOperationException("Hunger is not initialized.");
            var meal = _meals[playerId];
            var stage = meal.Missed >= def.CriticalAt ? HungerStage.Critical : meal.Missed >= def.StarvingAt
                ? HungerStage.Starving : meal.Missed > 0 ? HungerStage.Hungry : HungerStage.Fed;
            double speed = stage == HungerStage.Critical ? def.CriticalSpeed : stage == HungerStage.Starving ? def.StarvingSpeed : 1;
            return new(_mealDay, meal.Eaten, def.DailyFood, meal.Missed, stage, speed,
                checked(_mealDay * _mealClock!.CycleTicks), meal.Consumed, meal.MissedDeadlines, meal.SatisfiedDays);
        }
    }
    public EatResult Eat(int playerId, EatRequest request)
    {
        lock (_gate)
        {
            if (!_meals.TryGetValue(playerId, out var meal)) return new(EatStatus.InvalidPlayer, 0);
            if (request.Version != Version || request.Day != _mealDay) return new(EatStatus.Stale, 0);
            var player = _players[playerId];
            if (!player.Valid(request.Slot)) return new(EatStatus.InvalidSlot, 0);
            var stack = player.At(request.Slot);
            if (stack == null) return new(EatStatus.Empty, 0);
            if (!_resources[stack.ResourceId].Edible) return new(EatStatus.NotEdible, 0);
            if (meal.Eaten >= _hunger!.DailyFood) return new(EatStatus.Satisfied, 0);
            player.Take(request.Slot, 1); _revision++;
            Credit(meal, 1);
            return new(EatStatus.Eaten, 1);
        }
    }
    private void Credit(MealState meal, int amount)
    {
        meal.Eaten += amount; meal.Consumed += amount;
        // Once per completed daily requirement, never again when dawn settles that meal.
        if (meal.Eaten == _hunger!.DailyFood)
        {
            meal.Missed = Math.Max(0, meal.Missed - _hunger.RecoveryPerMeal);
            meal.SatisfiedDays++;
        }
    }
    private void SettleMeals(long elapsedTicks)
    {
        long day = elapsedTicks / _mealClock!.CycleTicks + 1;
        while (_mealDay < day)
        {
            foreach (var pair in _meals)
            {
                var meal = pair.Value; var player = _players[pair.Key];
                int remaining = _hunger!.DailyFood - meal.Eaten;
                // Stable catalog order; all edible units currently have the same meal value.
                foreach (var food in Resources.Where(r => r.Edible))
                {
                    int used = Math.Min(remaining, player.Count(food.Id));
                    if (used == 0) continue;
                    player.Remove(food.Id, used); _revision++;
                    Credit(meal, used); remaining -= used;
                }
                if (remaining > 0) { meal.Missed = checked(meal.Missed + 1); meal.MissedDeadlines++; }
                meal.Eaten = 0;
            }
            _mealDay++;
        }
    }
}
