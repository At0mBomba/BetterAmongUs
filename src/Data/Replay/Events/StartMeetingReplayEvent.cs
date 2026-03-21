using BetterAmongUs.Interfaces;
using System.Text.Json.Serialization;

namespace BetterAmongUs.Data.Replay.Events;

[Serializable]
internal sealed class StartMeetingReplayEvent : IReplayEvent<(int playerId, int targetId)>
{
    [JsonPropertyName("id")]
    public string Id => "start_meeting";

    [JsonPropertyName("eventData")]
    public (int playerId, int targetId) EventData { get; set; }

    public void Play()
    {
    }

    public void Record(PlayerControl player, PlayerControl? target)
    {
        EventData = (player.PlayerId, target?.PlayerId ?? -1);
    }
}
