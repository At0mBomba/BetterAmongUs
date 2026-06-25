using BetterAmongUs.Generated;
using BetterAmongUs.Modules;
using BetterAmongUs.Modules.OptionItems;
using BetterAmongUs.Modules.OptionItems.NoneOption;
using BetterAmongUs.Modules.Support;
using HarmonyLib;
using UnityEngine;

namespace BetterAmongUs.Patches.Gameplay.UI.Settings;

// Custom game settings definitions
internal sealed class BetterGameSettings
{
    internal static OptionStringItem? WhenCheating;
    internal static OptionCheckboxItem? InvalidFriendCode;
    internal static OptionCheckboxItem? UseBanPlayerList;
    internal static OptionCheckboxItem? UseBanNameList;
    internal static OptionCheckboxItem? UseBanWordList;
    internal static OptionCheckboxItem? UseBanWordListOnlyLobby;
    internal static OptionCheckboxItem? DetectedLevel;
    internal static OptionIntItem? DetectedLevelAbove;
    internal static OptionCheckboxItem? KickLevel;
    internal static OptionIntItem? KickLevelBelow;
    internal static OptionCheckboxItem? DetectCheatClients;
    internal static OptionCheckboxItem? DetectInvalidRpcs;
    internal static OptionCheckboxItem? RpcRateLimiting;
    internal static OptionIntItem? RpcRateLimit;

    internal static OptionStringItem? RoleRandomizer;

    internal static OptionCheckboxItem? CancelInvalidSabotage;
    internal static OptionCheckboxItem? CensorDetectionReason;
}

// Temporary settings for Hide & Seek impostor selection
/*
internal sealed class BetterGameSettingsTemp
{
    internal static OptionPlayerItem? HideAndSeekImp2;
    internal static OptionPlayerItem? HideAndSeekImp3;
    internal static OptionPlayerItem? HideAndSeekImp4;
    internal static OptionPlayerItem? HideAndSeekImp5;
}
*/

[HarmonyPatch]
internal static class GameSettingsPatch
{
    internal static OptionTab? BetterSettingsTab;

    // Creates all custom game settings and organizes them in tabs
    internal static void SetupSettings(bool IsPreload = false)
    {
        // Note: Use 2200 next ID

        // Create main settings tab
        BetterSettingsTab = OptionTab.Create(3, TranslationStrings.BetterSetting, TranslationStrings.BetterSetting_Description, Color.green);

        OptionHeaderItem.Create(BetterSettingsTab, TranslationStrings.BetterSetting_MainHeader_System);
        OptionPresetItem.Create();

        // Anti-Cheat Settings section
        {
            OptionHeaderItem.Create(BetterSettingsTab, TranslationStrings.BetterSetting_MainHeader_AntiCheat);

            // Host-only anti-cheat settings
            if (IsPreload || GameState.IsHost)
            {
                OptionTitleItem.Create(BetterSettingsTab, TranslationStrings.BetterSetting_TextHeader_HostOnly);
                BetterGameSettings.WhenCheating = OptionStringItem.Create(100, BetterSettingsTab, TranslationStrings.BetterSetting_Setting_WhenCheating,
                    [TranslationStrings.BetterSetting_Setting_WhenCheating_Notify, TranslationStrings.BetterSetting_Setting_WhenCheating_Kick, TranslationStrings.BetterSetting_Setting_WhenCheating_Ban], 2);
                BetterGameSettings.InvalidFriendCode = OptionCheckboxItem.Create(200, BetterSettingsTab, TranslationStrings.BetterSetting_Setting_InvalidFriendCode, true);
                BetterGameSettings.CancelInvalidSabotage = OptionCheckboxItem.Create(900, BetterSettingsTab, TranslationStrings.BetterSetting_Setting_CancelInvalidSabotage, true);
                BetterGameSettings.UseBanPlayerList = OptionCheckboxItem.Create(300, BetterSettingsTab, TranslationStrings.BetterSetting_Setting_UseBanPlayerList, true);
                BetterGameSettings.UseBanNameList = OptionCheckboxItem.Create(400, BetterSettingsTab, TranslationStrings.BetterSetting_Setting_UseBanNameList, true);
                BetterGameSettings.UseBanWordList = OptionCheckboxItem.Create(500, BetterSettingsTab, TranslationStrings.BetterSetting_Setting_UseBanWordList, true);
                BetterGameSettings.UseBanWordListOnlyLobby = OptionCheckboxItem.Create(1400, BetterSettingsTab, TranslationStrings.BetterSetting_Setting_UseBanWordListOnlyLobby, true, BetterGameSettings.UseBanWordList);
            }

            // General detection settings
            OptionTitleItem.Create(BetterSettingsTab, TranslationStrings.BetterSetting_TextHeader_Detections);
            BetterGameSettings.CensorDetectionReason = OptionCheckboxItem.Create(1300, BetterSettingsTab, TranslationStrings.BetterSetting_Setting_CensorDetectionReason, false);
            BetterGameSettings.DetectedLevel = OptionCheckboxItem.Create(1800, BetterSettingsTab, TranslationStrings.BetterSetting_Setting_DetectedLevel, false);
            BetterGameSettings.DetectedLevelAbove = OptionIntItem.Create(600, BetterSettingsTab, TranslationStrings.BetterSetting_Setting_DetectedLevelAbove, (100, 10000, 5), 500, ("Lv ", ""), BetterGameSettings.DetectedLevel);
            BetterGameSettings.KickLevel = OptionCheckboxItem.Create(1900, BetterSettingsTab, TranslationStrings.BetterSetting_Setting_KickLevel, false);
            BetterGameSettings.KickLevelBelow = OptionIntItem.Create(1700, BetterSettingsTab, TranslationStrings.BetterSetting_Setting_KickLevelBelow, (0, 10000, 1), 0, ("Lv ", ""), BetterGameSettings.KickLevel);
            BetterGameSettings.DetectCheatClients = OptionCheckboxItem.Create(700, BetterSettingsTab, TranslationStrings.BetterSetting_Setting_DetectCheatClients, true);
            BetterGameSettings.DetectInvalidRpcs = OptionCheckboxItem.Create(800, BetterSettingsTab, TranslationStrings.BetterSetting_Setting_DetectInvalidRpcs, true);
            BetterGameSettings.RpcRateLimiting = OptionCheckboxItem.Create(2000, BetterSettingsTab, TranslationStrings.BetterSetting_Setting_RpcRateLimiting, true);
            BetterGameSettings.RpcRateLimit = OptionIntItem.Create(2100, BetterSettingsTab, TranslationStrings.BetterSetting_Setting_RateLimit, (25, 1000, 1), 50, ("", " PS"), BetterGameSettings.RpcRateLimiting);
        }

        // Role algorithm settings
        if (IsPreload || GameState.IsHost)
        {
            OptionHeaderItem.Create(BetterSettingsTab, TranslationStrings.BetterSetting_MainHeader_RoleAlgorithm);
            BetterGameSettings.RoleRandomizer = OptionStringItem.Create(1100, BetterSettingsTab, TranslationStrings.BetterSetting_Setting_RoleRandomizer, [new("System.Random"), new("UnityEngine.Random")], 0);
            // BetterGameSettings.DesyncRoles = OptionCheckboxItem.Create(1200, BetterSettingsTab, "BetterSetting.Setting.DesyncRoles", true);
        }

        // Gameplay Settings section
        {
            // Show only for hosts
            if (IsPreload || GameState.IsHost)
            {
                /*
                // Normal game mode settings
                if (IsPreload || !GameState.IsHideNSeek)
                {
                    OptionHeaderItem.Create(BetterSettingsTab, "BetterSetting.MainHeader.Gameplay");
                    BetterGameSettings.DisableSabotages = OptionCheckboxItem.Create(1500, BetterSettingsTab, "BetterSetting.Setting.DisableSabotages", false);
                    BetterGameSettings.DisableSabotagesForDead = OptionCheckboxItem.Create(1600, BetterSettingsTab, "BetterSetting.Setting.DisableSabotagesForDead", false);
                }

                // Hide & Seek specific settings with dynamic impostor selection
                if (IsPreload || GameState.IsHideNSeek)
                {
                    OptionHeaderItem.Create(BetterSettingsTab, "BetterSetting.MainHeader.HideNSeek");
                    OptionTitleItem.Create(BetterSettingsTab, $"<color={RoleTypes.Impostor.GetRoleHex()}>{Translator.GetString(StringNames.ImpostorsCategory)}</color>");
                    BetterGameSettings.HideAndSeekImpNum = OptionIntItem.Create(1000, BetterSettingsTab, "BetterSetting.Setting.HideAndSeekImpNum", (1, 5, 1), 1);

                    // Create player selection dropdowns that appear based on impostor count
                    BetterGameSettingsTemp.HideAndSeekImp2 = OptionPlayerItem.Create(0, BetterSettingsTab, "BetterSetting.TempSetting.HideAndSeekImpNum", BetterGameSettings.HideAndSeekImpNum);
                    BetterGameSettingsTemp.HideAndSeekImp2.ShowCondition = () => BetterGameSettings.HideAndSeekImpNum.GetValue() >= 2;

                    BetterGameSettingsTemp.HideAndSeekImp3 = OptionPlayerItem.Create(1, BetterSettingsTab, "BetterSetting.TempSetting.HideAndSeekImpNum", BetterGameSettings.HideAndSeekImpNum);
                    BetterGameSettingsTemp.HideAndSeekImp3.ShowCondition = () => BetterGameSettings.HideAndSeekImpNum.GetValue() >= 3 &&
                    BetterGameSettingsTemp.HideAndSeekImp2.GetValue() != -1;

                    BetterGameSettingsTemp.HideAndSeekImp4 = OptionPlayerItem.Create(2, BetterSettingsTab, "BetterSetting.TempSetting.HideAndSeekImpNum", BetterGameSettings.HideAndSeekImpNum);
                    BetterGameSettingsTemp.HideAndSeekImp4.ShowCondition = () => BetterGameSettings.HideAndSeekImpNum.GetValue() >= 4 &&
                    BetterGameSettingsTemp.HideAndSeekImp2.GetValue() != -1 &&
                    BetterGameSettingsTemp.HideAndSeekImp3.GetValue() != -1;

                    BetterGameSettingsTemp.HideAndSeekImp5 = OptionPlayerItem.Create(3, BetterSettingsTab, "BetterSetting.TempSetting.HideAndSeekImpNum", BetterGameSettings.HideAndSeekImpNum);
                    BetterGameSettingsTemp.HideAndSeekImp5.ShowCondition = () => BetterGameSettings.HideAndSeekImpNum.GetValue() >= 5 &&
                    BetterGameSettingsTemp.HideAndSeekImp2.GetValue() != -1 &&
                    BetterGameSettingsTemp.HideAndSeekImp3.GetValue() != -1 &&
                    BetterGameSettingsTemp.HideAndSeekImp4.GetValue() != -1;
                }
                */
            }
        }

        BetterSettingsTab.UpdateVisuals();
    }

    // Initialize settings
    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
    [HarmonyPostfix]
    private static void GameSettingMenu_Start_Postfix(GameSettingMenu __instance)
    {
        if (BAUModdedSupportFlags.HasFlag(BAUModdedSupportFlags.Disable_AllGameOptions))
            return;

        SetupSettings();

        // Adjust menu layout
        __instance.gameObject.transform.SetLocalY(-0.1f);
        GameObject PanelSprite = __instance.gameObject.transform.Find("PanelSprite").gameObject;
        if (PanelSprite != null)
        {
            PanelSprite.transform.SetLocalY(-0.32f);
            PanelSprite.transform.localScale = new Vector3(PanelSprite.transform.localScale.x, 0.625f);
        }

        // Clear hover effects
        __instance.GamePresetsButton.OnMouseOver.RemoveAllListeners();
        __instance.GameSettingsButton.OnMouseOver.RemoveAllListeners();
        __instance.RoleSettingsButton.OnMouseOver.RemoveAllListeners();

        // Configure tab behavior based on game mode and host status
        BetterSettingsTab.TabButton.transform.localPosition = BetterSettingsTab.TabButton.transform.localPosition - new Vector3(0f, 1.265f, 0f);

        if (!GameState.IsHideNSeek && GameState.IsHost)
        {
            __instance.ChangeTab(1, false); // Show game settings tab for hosts
        }
        else if (GameState.IsHost)
        {
            // Hide & Seek host: disable role button but keep visible
            __instance.RoleSettingsButton.gameObject.SetActive(true);
            __instance.RoleSettingsButton.GetComponent<PassiveButton>().enabled = false;
            __instance.RoleSettingsButton.inactiveSprites.GetComponent<SpriteRenderer>().color = new(0.5f, 0.5f, 0.5f, 1f);
            __instance.ChangeTab(1, false);
        }
        else
        {
            // Non-hosts: disable all vanilla tabs and show custom tab
            __instance.GamePresetsButton.GetComponent<PassiveButton>().enabled = false;
            __instance.GameSettingsButton.GetComponent<PassiveButton>().enabled = false;
            __instance.RoleSettingsButton.GetComponent<PassiveButton>().enabled = false;
            __instance.GamePresetsButton.inactiveSprites.GetComponent<SpriteRenderer>().color = new(0.5f, 0.5f, 0.5f, 1f);
            __instance.GameSettingsButton.inactiveSprites.GetComponent<SpriteRenderer>().color = new(0.5f, 0.5f, 0.5f, 1f);
            __instance.RoleSettingsButton.inactiveSprites.GetComponent<SpriteRenderer>().color = new(0.5f, 0.5f, 0.5f, 1f);
            __instance.ChangeTab(3, false); // Switch to custom tab
        }
    }

    // Handle tab switching to show/hide custom settings tab
    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.ChangeTab))]
    [HarmonyPrefix]
    private static void GameSettingMenu_ChangeTab_Prefix(GameSettingMenu __instance, [HarmonyArgument(0)] int tabNum, [HarmonyArgument(1)] bool previewOnly)
    {
        if (BAUModdedSupportFlags.HasFlag(BAUModdedSupportFlags.Disable_AllGameOptions))
            return;

        if (BetterSettingsTab == null || BetterSettingsTab.AUTab == null || BetterSettingsTab.TabButton == null)
            return;

        BetterSettingsTab.AUTab.gameObject.SetActive(false);
        BetterSettingsTab.TabButton.SelectButton(false);

        // Show custom tab when selected (tab 3)
        if (previewOnly && Controller.currentTouchType == Controller.TouchType.Joystick || !previewOnly)
        {
            switch (tabNum)
            {
                case 3:
                    BetterSettingsTab.AUTab.gameObject.SetActive(true);
                    BetterSettingsTab.TabButton.SelectButton(true);
                    __instance.MenuDescriptionText.text = BetterSettingsTab.Description;
                    break;
            }
        }
    }

    // Prevent vanilla settings from being created in custom tab
    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.CreateSettings))]
    [HarmonyPrefix]
    private static bool GameOptionsMenu_CreateSettings_Prefix(GameOptionsMenu __instance)
    {
        if (BAUModdedSupportFlags.HasFlag(BAUModdedSupportFlags.Disable_AllGameOptions)) return true;

        // Skip creation if this is our custom tab
        if (__instance == BetterSettingsTab.AUTab)
        {
            return false;
        }

        return true;
    }

    // Make OptionsConsole usable by anyone (not just host)
    [HarmonyPatch(typeof(OptionsConsole), nameof(OptionsConsole.CanUse))]
    [HarmonyPrefix]
    private static void OptionsConsole_CanUse_Prefix(OptionsConsole __instance)
    {
        if (BAUModdedSupportFlags.HasFlag(BAUModdedSupportFlags.Disable_AllGameOptions))
            return;

        __instance.HostOnly = false; // Allow non-hosts to use settings console
    }
}