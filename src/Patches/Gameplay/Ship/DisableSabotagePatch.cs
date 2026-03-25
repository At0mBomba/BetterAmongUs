using BetterAmongUs.Helpers;
using BetterAmongUs.Modules;
using BetterAmongUs.Patches.Gameplay.UI.Settings;
using HarmonyLib;

namespace BetterAmongUs.Patches.Gameplay.Ship;

[HarmonyPatch]
internal static class DisableSabotagePatch
{
    [HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageReactor))]
    [HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageHeli))]
    [HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageComms))]
    [HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageOxygen))]
    [HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageLights))]
    [HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageSeismic))]
    [HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageMushroomMixup))]
    [HarmonyPrefix]
    internal static bool MapRoom_Sabotage_Prefix()
    {
        if (GameState.IsHost)
        {
            if (BetterGameSettings.DisableSabotages.GetBool())
                return false;

            if (BetterGameSettings.DisableSabotagesForDead.GetBool())
            {
                if (!PlayerControl.LocalPlayer.IsAlive())
                {
                    return false;
                }
            }
        }

        return true;
    }
}
