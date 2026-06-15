global using BTD_Mod_Helper.Extensions;
using System;
using AllLuck;
using BTD_Mod_Helper;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Simulation;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using MelonLoader;

[assembly: MelonInfo(typeof(AllLuck.AllLuck), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6-Epic")]

namespace AllLuck;

public partial class AllLuck : BloonsTD6Mod
{
    public override void OnApplicationStart()
    {
        ModHelper.Msg<AllLuck>("AllLuck loaded! Every game is now decided by the dice of fate.");
    }

    public override void OnNewGameModel(GameModel result)
    {
        if (!Enabled) return;

        RollSeedIfNeeded();

        try
        {
            if (RandomizeTowerStats) RandomizeAllTowers(result);
        }
        catch (Exception e)
        {
            ModHelper.Warning<AllLuck>($"AllLuck failed to randomize tower stats: {e}");
        }

        try
        {
            if (RandomizeBloons) RandomizeAllRounds(result);
        }
        catch (Exception e)
        {
            ModHelper.Warning<AllLuck>($"AllLuck failed to randomize bloon spawns: {e}");
        }

        try
        {
            if (RandomizeBloonSpeed) RandomizeBloonSpeeds(result);
        }
        catch (Exception e)
        {
            ModHelper.Warning<AllLuck>($"AllLuck failed to randomize bloon speed: {e}");
        }

        try
        {
            if (RandomizeTowerCost) RandomizeTowerCosts(result);
        }
        catch (Exception e)
        {
            ModHelper.Warning<AllLuck>($"AllLuck failed to randomize tower costs: {e}");
        }

        try
        {
            if (RandomizeUpgradeCost) RandomizeUpgradeCosts(result);
        }
        catch (Exception e)
        {
            ModHelper.Warning<AllLuck>($"AllLuck failed to randomize upgrade costs: {e}");
        }
    }

    public override void OnRoundEnd()
    {
        if (!Enabled || !RandomizeCash) return;

        try
        {
            GrantLuckyCash();
        }
        catch (Exception e)
        {
            ModHelper.Warning<AllLuck>($"AllLuck failed to roll end-of-round cash: {e}");
        }
    }

    public override void OnCashAdded(double amount, Simulation.CashType from, int cashIndex,
        Simulation.CashSource source, Tower tower)
    {
        if (!Enabled || !RandomizeIncome) return;

        try
        {
            var extra = IncomeBonus(amount, source.ToString());
            var inGame = InGame.instance;
            if (inGame != null && Math.Abs(extra) > 0.0001) inGame.AddCash(extra);
        }
        catch (Exception e)
        {
            ModHelper.Warning<AllLuck>($"AllLuck failed to apply income multiplier: {e}");
        }
    }

    public override void OnMatchStart()
    {
        if (Enabled) armLivesScaling = true;
    }

    public override void OnRoundStart()
    {
        if (!Enabled) return;

        try
        {
            ApplyStartingLivesIfArmed();
        }
        catch (Exception e)
        {
            ModHelper.Warning<AllLuck>($"AllLuck failed to roll starting lives: {e}");
        }
    }

    public override void OnMatchEnd()
    {
        hasRolled = false;
        armLivesScaling = false;
    }

    public override void OnRestart()
    {
        hasRolled = false;
        armLivesScaling = true;
    }
}
