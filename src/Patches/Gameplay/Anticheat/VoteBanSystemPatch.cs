using BetterAmongUs.Helpers;
using BetterAmongUs.Managers;
using BetterAmongUs.Modules;
using BetterAmongUs.Modules.Support;
using HarmonyLib;

namespace BetterAmongUs.Patches.Gameplay.Anticheat;

[HarmonyPatch]
internal static class VoteBanSystemPatch
{
    private static readonly List<(int ClientId, (ushort HashPuid, string FriendCode) Voter)> _voteData = [];

    [HarmonyPatch(typeof(VoteBanSystem), nameof(VoteBanSystem.Awake))]
    [HarmonyPrefix]
    private static void VoteBanSystem_Awake_Prefix()
    {
        _voteData.Clear();
    }


    [HarmonyPatch(typeof(VoteBanSystem), nameof(VoteBanSystem.AddVote))]
    [HarmonyPrefix]
    private static bool VoteBanSystem_AddVote_Prefix(VoteBanSystem __instance, int srcClient, int clientId, ref bool __state)
    {
        __state = false;

        // Skip BAU anti-cheat if disabled by other mods
        if (BAUModdedSupportFlags.HasFlag(BAUModdedSupportFlags.Disable_Anticheat)) return true;

        // If not host, allow vote and log it
        if (!GameState.IsHost)
        {
            __state = true;
            return true;
        }

        var client = Utils.ClientFromClientId(srcClient);
        if (client == null) return false;

        // Allow host to vote without restrictions
        if (client.Id == AmongUsClient.Instance.GetHost().Id)
        {
            return true;
        }

        void TryFlagPlayer()
        {
            var player = client.Character;
            if (player != null)
            {
                BetterNotificationManager.NotifyCheat(player, string.Format(Translator.GetString("AntiCheat.InvalidLobbyRPC"), "VoteKick"));
            }
        }

        // Prevent voting in lobby (anti-cheat measure)
        if (GameState.IsLobby)
        {
            TryFlagPlayer();
            return false;
        }

        // If client has no ID, allow vote but log it
        if (string.IsNullOrEmpty(client.ProductUserId) && string.IsNullOrEmpty(client.FriendCode))
        {
            __state = true;
            return true;
        }

        // Generate hash for client's ProductUserId
        var clientHash = Utils.GetHashUInt16(client.ProductUserId);

        // Check if this client has already voted for the same target
        foreach (var (targetClientId, (existingHash, existingFriendCode)) in _voteData)
        {
            if (targetClientId != clientId)
                continue;

            // Detect duplicate votes by comparing hash or friend code
            bool isDuplicateVote = existingHash == clientHash ||
                                  !string.IsNullOrEmpty(client.FriendCode) &&
                                   existingFriendCode == client.FriendCode;

            if (isDuplicateVote)
            {
                // Block duplicate vote attempt
                return false;
            }
        }

        // Record the vote
        _voteData.Add((clientId, (clientHash, client.FriendCode)));
        __state = true;
        return true;
    }

    [HarmonyPatch(typeof(VoteBanSystem), nameof(VoteBanSystem.AddVote))]
    [HarmonyPostfix]
    private static void VoteBanSystem_AddVote_Postfix(VoteBanSystem __instance, int srcClient, int clientId, bool __state)
    {
        // Skip logging if anti-cheat disabled
        if (BAUModdedSupportFlags.HasFlag(BAUModdedSupportFlags.Disable_Anticheat))
            return;

        // Log the vote if it was allowed
        if (__state)
        {
            LogVote(__instance, srcClient, clientId);
        }
    }

    private static void LogVote(VoteBanSystem voteBanSystem, int srcClient, int clientId)
    {
        // Get source and target client info
        var src = Utils.ClientFromClientId(srcClient);
        var client = Utils.ClientFromClientId(clientId);

        // Calculate current votes and required votes
        int currentVotes = 0;
        int maxVotes = 0;

        if (voteBanSystem.Votes.TryGetValue(clientId, out var votes))
        {
            currentVotes = votes.Count(v => v != 0); // Count non-zero votes
            maxVotes = votes.Length; // Total possible votes
        }

        // Log vote with colored player names and vote count
        Logger_.InGame(
            $"{src.Character?.GetPlayerNameAndColor() ?? src.PlayerName} " +
            $"voted to kick {client.Character?.GetPlayerNameAndColor() ?? client.PlayerName} " +
            $"<#6F6F6F>(</color><#FFFFFF>{currentVotes}</color><#6F6F6F>/</color><#FFFFFF>{maxVotes}</color><#6F6F6F>)</color>"
        );
    }
}