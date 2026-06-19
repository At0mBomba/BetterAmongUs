using AmongUs.GameOptions;
using BetterAmongUs.Utilities;
using BetterAmongUs.Interfaces;
using System.Text.Json.Serialization;

namespace BetterAmongUs.Data.Replay.Events;

[Serializable]
internal sealed class SetRoleEvent : IReplayEvent<(int playerId, int roleType)>
{
    [JsonPropertyName("id")]
    public string Id => "set_role";

    [JsonPropertyName("eventData")]
    public (int playerId, int roleType) EventData { get; set; }

    public void Play()
    {
        var player = Utils.PlayerFromPlayerId(EventData.playerId);
        if (player != null)
        {
            var roleType = (RoleTypes)EventData.roleType;
            player.RpcSetRole(roleType, true);
        }
    }

    public void Record(PlayerControl player, RoleTypes roleType)
    {
        EventData = (player.Data.PlayerId, (int)roleType);
    }
}
