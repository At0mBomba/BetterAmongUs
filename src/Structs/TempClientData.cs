using BetterAmongUs.Helpers;
using InnerNet;

namespace BetterAmongUs.Structs;

/// <summary>
/// Represents temporary client data for caching or snapshot purposes.
/// </summary>
/// <param name="clientData">The client data to copy values from.</param>
internal readonly struct TempClientData(ClientData clientData)
{
    /// <summary>
    /// Gets the unique identifier of the client.
    /// </summary>
    internal readonly int Id = clientData.Id;

    /// <summary>
    /// Gets the product user ID (PUID) of the client.
    /// </summary>
    internal readonly string Puid = clientData.ProductUserId;

    /// <summary>
    /// Gets the hashed product user ID.
    /// </summary>
    internal readonly string HashPuid = Utils.GetHashStr(clientData.ProductUserId);

    /// <summary>
    /// Gets the friend code of the client.
    /// </summary>
    internal readonly string FriendCode = clientData.FriendCode;

    /// <summary>
    /// Gets the player name of the client.
    /// </summary>
    internal readonly string PlayerName = clientData.PlayerName;
}