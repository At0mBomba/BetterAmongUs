using BetterAmongUs.Utilities;
using BetterAmongUs.Interfaces;
using System.Text.Json.Serialization;

namespace BetterAmongUs.Data.Replay.Events;

[Serializable]
internal sealed class UpdateSystemReplayEvent : IReplayEvent<(byte systemType, int playerId, byte amount)>
{
    [JsonPropertyName("id")]
    public string Id => "update_system";

    [JsonPropertyName("eventData")]
    public (byte systemType, int playerId, byte amount) EventData { get; set; }

    public void Play()
    {
        var player = Utils.PlayerFromPlayerId(EventData.playerId);
        if (player == null)
            return;

        if (ShipStatus.Instance == null)
            return;

        ShipStatus.Instance?.UpdateSystem((SystemTypes)EventData.systemType, player, EventData.amount);
    }

    public void Record(SystemTypes system, PlayerControl player, byte amount)
    {
        EventData = (checked((byte)system), player.PlayerId, amount);
    }
}
