using System;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using Il2CppAssets.Scripts.Unity.UI_New.Pause;
using Il2CppTMPro;
using TextCopy;
using UnityEngine;

namespace AllLuck;

public partial class AllLuck
{
    private const string SeedDisplayName = "AllLuckSeedDisplay";
    private const string CopyButtonName = "AllLuckCopyButton";

    public override void OnPauseScreenOpened(PauseScreen pauseScreen)
    {
        if (!Enabled) return;

        try
        {
            ShowSeedOnPauseScreen(pauseScreen);
        }
        catch (Exception e)
        {
            ModHelper.Warning<AllLuck>($"AllLuck failed to show the seed on the pause screen: {e}");
        }
    }

    private static void ShowSeedOnPauseScreen(PauseScreen pauseScreen)
    {
        var root = pauseScreen.gameObject;

        DestroyExisting(root, SeedDisplayName);
        DestroyExisting(root, CopyButtonName);

        var seedLabel = hasRolled ? activeSeed.ToString() : "—";

        var text = root.AddText(
            new Info(SeedDisplayName, 0, 150, 1600, 110, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f)),
            $"AllLuck Seed: {seedLabel}",
            55f,
            TextAlignmentOptions.Center);
        text.Text.color = new Color(1f, 0.85f, 0.2f);

        ModHelperText buttonLabel = null;
        var button = root.AddButton(
            new Info(CopyButtonName, 0, 45, 380, 95, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f)),
            VanillaSprites.GreenBtnLong,
            () =>
            {
                try
                {
                    ClipboardService.SetText(activeSeed.ToString());
                    buttonLabel?.SetText("Copied!");
                    ModHelper.Msg<AllLuck>($"Copied seed {activeSeed} to clipboard.");
                }
                catch (Exception ex)
                {
                    buttonLabel?.SetText("Copy failed");
                    ModHelper.Warning<AllLuck>($"AllLuck couldn't copy the seed: {ex}");
                }
            });

        buttonLabel = button.AddText(new Info("Label", InfoPreset.FillParent), "Copy Seed", 42f,
            TextAlignmentOptions.Center);
    }

    private static void DestroyExisting(GameObject root, string name)
    {
        var existing = root.transform.Find(name);
        if (existing != null) UnityEngine.Object.Destroy(existing.gameObject);
    }
}
