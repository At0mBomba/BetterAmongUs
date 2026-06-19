using AmongUs.GameOptions;
using BetterAmongUs.Utilities;
using BetterAmongUs.Interfaces;
using System.Text.Json.Serialization;

namespace BetterAmongUs.Data.Replay.Events;

[Serializable]
internal sealed class VanishReplayEvent : IReplayEvent<int>
{
    [JsonPropertyName("id")]
    public string Id => "player_vanish";

    [JsonPropertyName("eventData")]
    public int EventData { get; set; }

    public void Play()
    {
        var player = Utils.PlayerFromPlayerId(EventData);
        if (player == null)
            return;

        if (player.Data.RoleType != RoleTypes.Phantom)
            return;

        player.SetRoleInvisibility(true, true, true);
    }

    public void Record(PlayerControl player)
    {
        EventData = player.PlayerId;
    }
}
