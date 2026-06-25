using BetterAmongUs.Interfaces;
using BetterAmongUs.Utilities;
using System.Text.Json.Serialization;

namespace BetterAmongUs.Data.Replay.Events;

[Serializable]
internal sealed class MurderReplayEvent : IReplayEvent<MurderReplayEvent.MurderReplayData, MurderReplayEvent.MurderReplayArgs>
{
    [JsonPropertyName("id")]
    public string Id => "player_murder";

    [JsonPropertyName("eventData")]
    public MurderReplayData? EventData { get; set; }

    public void Play()
    {
        var killer = Utils.PlayerFromPlayerId(EventData.killerId);
        if (killer == null)
            return;

        var target = Utils.PlayerFromPlayerId(EventData.targetId);
        if (target == null)
            return;

        killer.MurderPlayer(target, MurderResultFlags.Succeeded);
    }

    public void Undo()
    {
    }

    public void Record(MurderReplayArgs murderReplayArgs)
    {
        EventData = new MurderReplayData(murderReplayArgs.killer.PlayerId, murderReplayArgs.target.PlayerId);
    }

    internal record MurderReplayData(int killerId, int targetId) : IReplayEvent.Data;

    internal record MurderReplayArgs(PlayerControl killer, PlayerControl target) : IReplayEvent.Args;
}
