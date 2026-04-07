using HarmonyLib;

namespace BetterAmongUs.Patches.Client;

[HarmonyPatch]
internal static class AudioOverridePatch
{
    [HarmonyPatch(typeof(SoundStarter), nameof(SoundStarter.Awake))]
    [HarmonyPrefix]
    private static void SoundStarter_Awake_Prefix(SoundStarter __instance)
    {
        if (__instance.gameObject.name == "MainMenuBgMusic")
        {
            if (AudioOverrideManager.Music_MainMenuSong.TryGetAudioClip(out var audio))
            {
                __instance.SoundToPlay = audio;
            }
        }
    }

    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    [HarmonyPrefix]
    private static void LobbyBehaviour_Start_Prefix(LobbyBehaviour __instance)
    {
        if (AudioOverrideManager.Music_LobbySong.TryGetAudioClip(out var audio))
        {
            __instance.MapTheme = audio;
        }
    }

    [HarmonyPatch(typeof(MeetingCalledAnimation), nameof(MeetingCalledAnimation.CoShow))]
    [HarmonyPrefix]
    private static void MeetingCalledAnimation_CoShow_Prefix(MeetingCalledAnimation __instance)
    {
        if (AudioOverrideManager.Sounds_EmergencyMeeting.TryGetAudioClip(out var audio))
        {
            __instance.Stinger = audio;
        }
    }
}
