using System;
using System.Collections.Generic;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Enums;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;

namespace AllLuck;

public partial class AllLuck
{

    private static ulong activeSeed;
    private static bool hasRolled;

    private static double incomeMultiplier = 1.0;

    private static bool armLivesScaling;

    private static readonly Dictionary<string, string> originalBloons = new();

    private static void RollSeedIfNeeded()
    {
        if (hasRolled) return;

        if (RandomizeEachGame)
        {
            activeSeed = unchecked((ulong) Guid.NewGuid().GetHashCode() * 0x9E3779B97F4A7C15UL ^
                                   (ulong) DateTime.UtcNow.Ticks);
        }
        else
        {
            activeSeed = ComputeSeedFromString(Seed);
        }

        hasRolled = true;

        incomeMultiplier = RandomizeIncome ? RollGlobal("global|income", IncomeMin, IncomeMax) : 1.0;

        ModHelper.Msg<AllLuck>(
            $"AllLuck seed for this game: {activeSeed}  " +
            "(paste this number into the Seed setting with 'Randomize Each Game' off to replay it)");
    }

    private static ulong ComputeSeedFromString(string s)
    {
        s = (s ?? "").Trim();
        if (s.Length == 0) return 0UL;
        if (ulong.TryParse(s, out var u)) return u;
        if (long.TryParse(s, out var l)) return unchecked((ulong) l);
        return Hash64(s, 0xA5A5A5A5A5A5A5A5UL);
    }

    private static ulong Hash64(string s, ulong seed)
    {

        var h = 14695981039346656037UL ^ seed;
        foreach (var c in s)
        {
            h ^= c;
            h *= 1099511628211UL;
        }

        h += 0x9E3779B97F4A7C15UL;
        h = (h ^ (h >> 30)) * 0xBF58476D1CE4E5B9UL;
        h = (h ^ (h >> 27)) * 0x94D049BB133111EBUL;
        h ^= h >> 31;
        return h;
    }

    private static double Rand(string key)
    {
        return (Hash64(key, activeSeed) >> 11) * (1.0 / 9007199254740992.0);
    }

    private static double RandRange(string key, double min, double max) => min + Rand(key) * (max - min);

    private static double RollGlobal(string key, double min, double max)
    {
        min = Clamp(min, 0.01, 100);
        max = Clamp(max, min, 100);
        return RandRange(key, min, max);
    }

    private static double Clamp(double v, double lo, double hi) => v < lo ? lo : v > hi ? hi : v;
    private static int Clamp(int v, int lo, int hi) => v < lo ? lo : v > hi ? hi : v;

    private static void RandomizeAllTowers(GameModel result)
    {
        var min = Clamp((double) TowerStatMin, 0.05, 100);
        var max = Clamp((double) TowerStatMax, min, 100);

        foreach (var tower in result.towers)
        {
            RandomizeTower(tower, min, max);
        }
    }

    private static void RandomizeTower(TowerModel tower, double min, double max)
    {

        var id = string.IsNullOrEmpty(tower.baseId) ? tower.name : tower.baseId;

        var damageMult = (float) RandRange($"tower|{id}|damage", min, max);
        var pierceMult = (float) RandRange($"tower|{id}|pierce", min, max);
        var rangeMult = (float) RandRange($"tower|{id}|range", min, max);
        var speedMult = (float) RandRange($"tower|{id}|speed", min, max);

        tower.GetDescendants<DamageModel>().ForEach(dm =>
        {
            dm.damage *= damageMult;
            dm.maxDamage *= damageMult;
        });

        tower.GetDescendants<ProjectileModel>().ForEach(pm =>
        {
            pm.pierce *= pierceMult;
            pm.maxPierce *= pierceMult;
        });

        tower.range *= rangeMult;
        foreach (var attack in tower.GetAttackModels())
        {
            attack.range *= rangeMult;
        }

        foreach (var weapon in tower.GetWeapons())
        {
            if (weapon.rate > 0) weapon.rate /= speedMult;
        }
    }

    private static void RandomizeAllRounds(GameModel result)
    {
        var roundSet = result.roundSet;
        if (roundSet == null) return;

        var roundSetName = string.IsNullOrEmpty(roundSet.name) ? "default" : roundSet.name;
        var chaos = Clamp((int) BloonChaos, 0, 100);
        var scale = (bool) ScaleBloonsWithRound;

        var rounds = roundSet.rounds;
        for (var r = 0; r < rounds.Count; r++)
        {
            var round = rounds[r];
            if (round?.groups == null) continue;

            var displayRound = r + 1;
            var touched = false;

            for (var g = 0; g < round.groups.Count; g++)
            {
                var group = round.groups[g];
                if (group == null) continue;

                var key = $"{roundSetName}|{r}|{g}";
                if (!originalBloons.ContainsKey(key)) originalBloons[key] = group.bloon;
                var original = originalBloons[key];

                if (Rand($"bloon|swap|{key}") * 100.0 < chaos)
                {
                    group.bloon = PickBloon($"bloon|pick|{key}", displayRound, scale);
                }
                else
                {
                    group.bloon = original;
                }

                touched = true;
            }

            if (touched) round.emissions_ = null;
        }
    }

    private static string PickBloon(string key, int round, bool scale)
    {
        var pool = scale ? PoolForRound(round) : FullChaosPool;
        var total = 0;
        foreach (var entry in pool) total += entry.weight;

        var roll = Rand(key) * total;
        foreach (var entry in pool)
        {
            if (roll < entry.weight) return entry.id;
            roll -= entry.weight;
        }

        return pool[pool.Length - 1].id;
    }

    private static (string id, int weight)[] PoolForRound(int round)
    {
        if (round <= 6) return Tier0;
        if (round <= 12) return Tier1;
        if (round <= 22) return Tier2;
        if (round <= 39) return Tier3;
        if (round <= 59) return Tier4;
        if (round <= 79) return Tier5;
        return Tier6;
    }

    private static readonly (string id, int weight)[] Tier0 =
    {
        (BloonType.Red, 50), (BloonType.Blue, 26), (BloonType.Green, 13),
        (BloonType.Yellow, 7), (BloonType.Pink, 4)
    };

    private static readonly (string id, int weight)[] Tier1 =
    {
        (BloonType.Red, 22), (BloonType.Blue, 22), (BloonType.Green, 18), (BloonType.Yellow, 14),
        (BloonType.Pink, 10), (BloonType.White, 4), (BloonType.Black, 4), (BloonType.Lead, 3),
        (BloonType.Purple, 2), (BloonType.Zebra, 1)
    };

    private static readonly (string id, int weight)[] Tier2 =
    {
        (BloonType.Green, 14), (BloonType.Yellow, 16), (BloonType.Pink, 16), (BloonType.White, 9),
        (BloonType.Black, 9), (BloonType.Purple, 6), (BloonType.Lead, 6), (BloonType.Zebra, 8),
        (BloonType.Rainbow, 6), (BloonType.LeadCamo, 2), (BloonType.Ceramic, 2)
    };

    private static readonly (string id, int weight)[] Tier3 =
    {
        (BloonType.Pink, 10), (BloonType.White, 8), (BloonType.Black, 8), (BloonType.Purple, 7),
        (BloonType.Lead, 7), (BloonType.Zebra, 12), (BloonType.Rainbow, 16), (BloonType.Ceramic, 18),
        (BloonType.CeramicRegrow, 8), (BloonType.RainbowRegrow, 4), (BloonType.Moab, 2)
    };

    private static readonly (string id, int weight)[] Tier4 =
    {
        (BloonType.Rainbow, 14), (BloonType.Ceramic, 22), (BloonType.CeramicRegrow, 14),
        (BloonType.CeramicFortified, 8), (BloonType.ZebraRegrow, 6), (BloonType.Moab, 22),
        (BloonType.Bfb, 8), (BloonType.Ddt, 4), (BloonType.MoabFortified, 2)
    };

    private static readonly (string id, int weight)[] Tier5 =
    {
        (BloonType.Ceramic, 12), (BloonType.CeramicFortified, 12), (BloonType.Moab, 22),
        (BloonType.MoabFortified, 10), (BloonType.Bfb, 20), (BloonType.Ddt, 12),
        (BloonType.Zomg, 8), (BloonType.BfbFortified, 4)
    };

    private static readonly (string id, int weight)[] Tier6 =
    {
        (BloonType.Moab, 12), (BloonType.MoabFortified, 8), (BloonType.Bfb, 20),
        (BloonType.BfbFortified, 10), (BloonType.Ddt, 16), (BloonType.Zomg, 22),
        (BloonType.Bad, 8), (BloonType.BadFortified, 4)
    };

    private static readonly (string id, int weight)[] FullChaosPool =
    {
        (BloonType.Red, 16), (BloonType.Blue, 14), (BloonType.Green, 12), (BloonType.Yellow, 11),
        (BloonType.Pink, 10), (BloonType.White, 6), (BloonType.Black, 6), (BloonType.Purple, 5),
        (BloonType.Lead, 5), (BloonType.Zebra, 5), (BloonType.Rainbow, 5), (BloonType.Ceramic, 4),
        (BloonType.Moab, 3), (BloonType.Bfb, 2), (BloonType.Ddt, 2), (BloonType.Zomg, 1),
        (BloonType.Bad, 1)
    };

    private static void GrantLuckyCash()
    {
        var inGame = InGame.instance;
        if (inGame == null) return;

        var round = inGame.GetUnityToSimulation().GetCurrentRound();

        var min = Clamp((double) CashMin, 0, 100);
        var max = Clamp((double) CashMax, min, 100);

        var baseline = 75.0 + 8.0 * round;
        var bonus = Math.Round(baseline * RandRange($"cash|{round}", min, max));

        if (bonus > 0) inGame.AddCash(bonus);
    }

    private static void RandomizeBloonSpeeds(GameModel result)
    {
        if (result.bloons == null) return;

        var mult = (float) RollGlobal("global|bloonSpeed", BloonSpeedMin, BloonSpeedMax);

        foreach (var bloon in result.bloons)
        {
            if (bloon == null) continue;
            bloon.speed *= mult;
            bloon.speedFrames *= mult;
        }
    }

    private static void RandomizeTowerCosts(GameModel result)
    {
        if (result.towers == null) return;

        var mult = RollGlobal("global|towerCost", TowerCostMin, TowerCostMax);

        foreach (var tower in result.towers)
        {
            if (tower == null) continue;
            tower.cost = (int) Math.Max(0, Math.Round(tower.cost * mult));
        }
    }

    private static void RandomizeUpgradeCosts(GameModel result)
    {
        if (result.upgrades == null) return;

        var mult = RollGlobal("global|upgradeCost", UpgradeCostMin, UpgradeCostMax);

        foreach (var upgrade in result.upgrades)
        {
            if (upgrade == null) continue;
            upgrade.cost = (int) Math.Max(0, Math.Round(upgrade.cost * mult));
        }
    }

    private static void ApplyStartingLivesIfArmed()
    {
        if (!armLivesScaling) return;
        armLivesScaling = false;

        if (!RandomizeLives) return;

        RollSeedIfNeeded();

        var inGame = InGame.instance;
        if (inGame == null) return;

        var mult = RollGlobal("global|lives", LivesMin, LivesMax);
        var newLives = Math.Max(1, Math.Round(inGame.GetHealth() * mult));
        inGame.SetHealth(newLives);

        ModHelper.Msg<AllLuck>($"AllLuck rolled starting lives x{mult:0.00} -> {newLives}");
    }

    private static double IncomeBonus(double amount, string source)
    {
        if (!RandomizeIncome || amount <= 0) return 0;

        if (!string.IsNullOrEmpty(source) &&
            (source.Contains("Sell") || source.Contains("Sold"))) return 0;

        return amount * (incomeMultiplier - 1.0);
    }
}
