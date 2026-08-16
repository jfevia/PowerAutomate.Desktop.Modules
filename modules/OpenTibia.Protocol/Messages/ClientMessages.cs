// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages;

/// <summary>
/// Factory methods for payload-less client requests.
/// </summary>
public static class ClientMessages
{
    public static IClientMessage Logout() => new PayloadlessMessage(ClientOpcode.LeaveGame);
    public static IClientMessage PingBack() => new PayloadlessMessage(ClientOpcode.PingBack);
    public static IClientMessage RequestChannels() => new PayloadlessMessage(ClientOpcode.RequestChannels);
    public static IClientMessage RequestOutfit() => new PayloadlessMessage(ClientOpcode.RequestOutfit);
    public static IClientMessage RequestQuestLog() => new PayloadlessMessage(ClientOpcode.RequestQuestLog);
    public static IClientMessage CancelAttackAndFollow() => new PayloadlessMessage(ClientOpcode.CancelAttackAndFollow);
}
