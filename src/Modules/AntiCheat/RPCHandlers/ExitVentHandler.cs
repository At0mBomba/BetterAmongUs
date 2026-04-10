using AmongUs.GameOptions;
using BetterAmongUs.Attributes;
using BetterAmongUs.Helpers;
using BetterAmongUs.Managers;
using BetterAmongUs.Mono.Extended;
using Hazel;

namespace BetterAmongUs.Modules.AntiCheat.RPCHandlers;

[RegisterRPCHandler]
internal sealed class ExitVentHandler : RPCHandler
{
    internal override byte CallId => (byte)RpcCalls.ExitVent;

    internal override void HandleAntiCheat(PlayerControl? sender, MessageReader reader)
    {
        if (!sender.IsImpostorTeam() && !sender.Is(RoleTypes.Engineer))
        {
            if (BetterNotificationManager.NotifyCheat(sender, GetFormatActionText()))
            {
                LogRpcInfo($"Non-impostor and non-engineer attempted ExitVent RPC");
            }
        }
    }

    internal override void Handle(PlayerControl? sender, MessageReader reader)
    {
        sender.ExtendedData().GameplayInfo.InVent = false;
    }
}
