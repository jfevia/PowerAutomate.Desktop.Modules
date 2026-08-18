// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Services;
using PowerAutomate.Desktop.Modules.OpenTibia.Actions.Types;

namespace PowerAutomate.Desktop.Modules.OpenTibia.Actions.Actions.Client;

/// <summary>
/// Escape hatch to send an arbitrary opcode and hex-encoded payload not otherwise modelled.
/// </summary>
[Action(Id = "SendClientMessage", Category = Categories.ClientMessages)]
[Group(Name = Groups.General, Order = 1)]
[Group(Name = Groups.Advanced, Order = 2, IsDefault = true)]
[Throws(ErrorCodes.NotConnected + "Error")]
[Throws(ErrorCodes.InvalidArgument + "Error")]
[Throws(ErrorCodes.Unknown + "Error")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class SendClientMessageAction : OpenTibiaActionBase
{
    [InputArgument(Order = 1, Group = Groups.General)]
    public TibiaGameSession GameSession { get; set; } = null!;

    [InputArgument(Order = 2, Group = Groups.General)]
    public int Opcode { get; set; }

    [InputArgument(Order = 3, Required = false)]
    [DefaultValue("")]
    public string PayloadHex { get; set; } = string.Empty;

    protected override void Run(ActionContext context)
    {
        var session = SessionGuards.RequireInGame(GameSession);

        var opcode = RangeGuard.ToByte(Opcode, nameof(Opcode));
        var payload = HexCodec.Parse(PayloadHex);

        session.Client.Send(new RawClientMessage(opcode, payload));
    }
}
