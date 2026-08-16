// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Services;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Client;

[Action(Id = "SendTalk", Category = Categories.ClientMessages)]
[Group(Name = Groups.General, Order = 1)]
[Group(Name = Groups.Target, Order = 2)]
[Group(Name = Groups.Advanced, Order = 3, IsDefault = true)]
[Throws(ErrorCodes.NotConnected + "Error")]
[Throws(ErrorCodes.InvalidArgument + "Error")]
[Throws(ErrorCodes.ProtocolError + "Error")]
[Throws(ErrorCodes.Unknown + "Error")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class SendTalkAction : OpenTibiaActionBase
{
    [InputArgument(Order = 1, Group = Groups.General)]
    public TibiaGameSession GameSession { get; set; } = null!;

    [InputArgument(Order = 2, Group = Groups.General)]
    [DefaultValue(SpeakType.Say)]
    public SpeakType Type { get; set; } = SpeakType.Say;

    [InputArgument(Order = 3, Group = Groups.General)]
    public string Text { get; set; } = null!;

    [InputArgument(Order = 4, Group = Groups.Target, Required = false)]
    public string? ReceiverName { get; set; }

    [InputArgument(Order = 5, Group = Groups.Target, Required = false)]
    [DefaultValue(0)]
    public int ChannelId { get; set; }

    protected override void Run(ActionContext context)
    {
        var session = SessionGuards.RequireInGame(GameSession);

        var receiverName = string.IsNullOrEmpty(ReceiverName) ? null : ReceiverName;
        ushort? channelId = ChannelId > 0 ? (ushort)ChannelId : (ushort?)null;

        session.Client.Send(new ClientTalkMessage(Type, receiverName, channelId, Text ?? string.Empty));
    }
}
