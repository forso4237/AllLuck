using BTD_Mod_Helper.Api.ModOptions;

namespace AllLuck;

public partial class AllLuck
{
    private static readonly ModSettingCategory General = new("General")
    {
        collapsed = false
    };

    private static readonly ModSettingCategory Bloons = new("Bloon Spawns");
    private static readonly ModSettingCategory Towers = new("Tower Stats");
    private static readonly ModSettingCategory Cash = new("End of Round Cash");
    private static readonly ModSettingCategory Economy = new("Economy");
    private static readonly ModSettingCategory Rules = new("Game Rules");

    public static readonly ModSettingBool Enabled = new(true)
    {
        category = General,
        displayName = "Enable AllLuck",
        description = "Master switch. When off, the game plays completely normally."
    };

    public static readonly ModSettingString Seed = new("")
    {
        category = General,
        displayName = "Seed",
        description = "The seed that decides everything. Numbers reproduce exactly; any text works too. " +
                      "Only used when 'Randomize Each Game' is OFF."
    };

    public static readonly ModSettingBool RandomizeEachGame = new(true)
    {
        category = General,
        displayName = "Randomize Each Game",
        description = "When on, a brand-new random seed is rolled every match (and printed to the console / log). " +
                      "Turn this off and paste that number into 'Seed' to replay a run."
    };

    public static readonly ModSettingBool RandomizeBloons = new(true)
    {
        category = Bloons,
        displayName = "Randomize Bloon Spawns",
        description = "Re-rolls which bloon each group in each round sends out."
    };

    public static readonly ModSettingInt BloonChaos = new(60)
    {
        category = Bloons,
        displayName = "Bloon Chaos (%)",
        description = "Chance for each bloon group in a round to be swapped for a random bloon. " +
                      "0 = vanilla bloons, 100 = total chaos.",
        min = 0,
        max = 100,
        slider = true,
        sliderSuffix = "%"
    };

    public static readonly ModSettingBool ScaleBloonsWithRound = new(true)
    {
        category = Bloons,
        displayName = "Scale With Round",
        description = "Keeps random bloons appropriate to the round you're on (no ceramics on round 1). " +
                      "Turn off for true anything-goes mayhem."
    };

    public static readonly ModSettingBool RandomizeTowerStats = new(true)
    {
        category = Towers,
        displayName = "Randomize Tower Stats",
        description = "Each tower type gets its own lucky/unlucky multipliers for damage, pierce, range " +
                      "and attack speed for the whole game."
    };

    public static readonly ModSettingDouble TowerStatMin = new(0.5)
    {
        category = Towers,
        displayName = "Min Stat Multiplier",
        description = "Worst roll a stat can get (0.5 = half as good).",
        min = 0.05,
        max = 1,
        slider = true
    };

    public static readonly ModSettingDouble TowerStatMax = new(2.0)
    {
        category = Towers,
        displayName = "Max Stat Multiplier",
        description = "Best roll a stat can get (2.0 = twice as good).",
        min = 1,
        max = 5,
        slider = true
    };

    public static readonly ModSettingBool RandomizeCash = new(true)
    {
        category = Cash,
        displayName = "Randomize End of Round Cash",
        description = "Grants a seed-determined cash bonus at the end of each round. Some rounds you strike " +
                      "it rich, some you get crumbs."
    };

    public static readonly ModSettingDouble CashMin = new(0.25)
    {
        category = Cash,
        displayName = "Min Cash Multiplier",
        description = "Stingiest the bonus can be relative to the round's baseline.",
        min = 0,
        max = 1,
        slider = true
    };

    public static readonly ModSettingDouble CashMax = new(2.5)
    {
        category = Cash,
        displayName = "Max Cash Multiplier",
        description = "Most generous the bonus can be relative to the round's baseline.",
        min = 1,
        max = 10,
        slider = true
    };

    public static readonly ModSettingBool RandomizeBloonSpeed = new(true)
    {
        category = Bloons,
        displayName = "Randomize Bloon Speed",
        description = "Rolls one global speed multiplier for every bloon this game. Slow crawl or zoomies."
    };

    public static readonly ModSettingDouble BloonSpeedMin = new(0.7)
    {
        category = Bloons,
        displayName = "Min Bloon Speed",
        description = "Slowest bloons can roll (0.7 = 30% slower).",
        min = 0.2,
        max = 1,
        slider = true
    };

    public static readonly ModSettingDouble BloonSpeedMax = new(1.4)
    {
        category = Bloons,
        displayName = "Max Bloon Speed",
        description = "Fastest bloons can roll (1.4 = 40% faster).",
        min = 1,
        max = 3,
        slider = true
    };

    public static readonly ModSettingBool RandomizeIncome = new(true)
    {
        category = Economy,
        displayName = "Randomize Income",
        description = "Rolls one global multiplier on all cash you earn from popping bloons, eco, and bonuses " +
                      "(tower sell refunds are left alone)."
    };

    public static readonly ModSettingDouble IncomeMin = new(0.5)
    {
        category = Economy,
        displayName = "Min Income Multiplier",
        description = "Stingiest income can roll.",
        min = 0.1,
        max = 1,
        slider = true
    };

    public static readonly ModSettingDouble IncomeMax = new(2.0)
    {
        category = Economy,
        displayName = "Max Income Multiplier",
        description = "Most generous income can roll.",
        min = 1,
        max = 5,
        slider = true
    };

    public static readonly ModSettingBool RandomizeUpgradeCost = new(true)
    {
        category = Economy,
        displayName = "Randomize Upgrade Cost",
        description = "Rolls one global multiplier on the price of every tower upgrade."
    };

    public static readonly ModSettingDouble UpgradeCostMin = new(0.5)
    {
        category = Economy,
        displayName = "Min Upgrade Cost",
        description = "Cheapest upgrades can roll (0.5 = half price).",
        min = 0.1,
        max = 1,
        slider = true
    };

    public static readonly ModSettingDouble UpgradeCostMax = new(1.75)
    {
        category = Economy,
        displayName = "Max Upgrade Cost",
        description = "Most expensive upgrades can roll.",
        min = 1,
        max = 5,
        slider = true
    };

    public static readonly ModSettingBool RandomizeTowerCost = new(true)
    {
        category = Economy,
        displayName = "Randomize Base Tower Cost",
        description = "Rolls one global multiplier on the placement price of every tower."
    };

    public static readonly ModSettingDouble TowerCostMin = new(0.5)
    {
        category = Economy,
        displayName = "Min Tower Cost",
        description = "Cheapest towers can roll (0.5 = half price).",
        min = 0.1,
        max = 1,
        slider = true
    };

    public static readonly ModSettingDouble TowerCostMax = new(1.75)
    {
        category = Economy,
        displayName = "Max Tower Cost",
        description = "Most expensive towers can roll.",
        min = 1,
        max = 5,
        slider = true
    };

    public static readonly ModSettingBool RandomizeLives = new(true)
    {
        category = Rules,
        displayName = "Randomize Starting Lives",
        description = "Rolls a multiplier on your starting lives for the run. Never drops you below 1 life."
    };

    public static readonly ModSettingDouble LivesMin = new(0.5)
    {
        category = Rules,
        displayName = "Min Lives Multiplier",
        description = "Fewest starting lives can roll (0.5 = half).",
        min = 0.1,
        max = 1,
        slider = true
    };

    public static readonly ModSettingDouble LivesMax = new(1.5)
    {
        category = Rules,
        displayName = "Max Lives Multiplier",
        description = "Most starting lives can roll (1.5 = 50% more).",
        min = 1,
        max = 5,
        slider = true
    };
}
