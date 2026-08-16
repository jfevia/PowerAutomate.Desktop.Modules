// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Chat;

/// <summary>
/// A system/status text message's classification, matching TFS's MessageClasses enum values for protocol 8.60.
/// </summary>
public enum MessageClass : byte
{
    None = 0x00,
    StatusConsoleRed = 0x12,
    EventOrange = 0x13,
    StatusConsoleOrange = 0x14,
    StatusWarning = 0x15,
    EventAdvance = 0x16,
    EventDefault = 0x17,
    StatusDefault = 0x18,
    InfoDescr = 0x19,
    StatusSmall = 0x1A,
    StatusConsoleBlue = 0x1B
}
