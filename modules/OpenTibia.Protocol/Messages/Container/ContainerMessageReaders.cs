// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using PowerAutomate.Desktop.OpenTibia.Protocol.Items;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Container;

/// <summary>
/// Registers the container and equipped-inventory notice message readers.
/// </summary>
public static class ContainerMessageReaders
{
    /// <summary>
    /// Registers every opcode using <see cref="DefaultItemTypeProvider" />, since no items.otb ships with this module.
    /// </summary>
    public static void RegisterTo(GameServerMessageRegistry registry)
    {
        RegisterTo(registry, new DefaultItemTypeProvider());
    }

    public static void RegisterTo(GameServerMessageRegistry registry, IItemTypeProvider itemTypes)
    {
        if (registry == null)
        {
            throw new ArgumentNullException(nameof(registry));
        }

        if (itemTypes == null)
        {
            throw new ArgumentNullException(nameof(itemTypes));
        }

        registry.Register(GameServerOpcode.OpenContainer, (opcode, reader) => GameServerOpenContainerMessage.Read(opcode, reader, itemTypes));
        registry.Register(GameServerOpcode.CloseContainer, GameServerCloseContainerMessage.Read);
        registry.Register(GameServerOpcode.CreateContainer, (opcode, reader) => GameServerCreateContainerMessage.Read(opcode, reader, itemTypes));
        registry.Register(GameServerOpcode.ChangeInContainer, (opcode, reader) => GameServerChangeInContainerMessage.Read(opcode, reader, itemTypes));
        registry.Register(GameServerOpcode.DeleteInContainer, GameServerDeleteInContainerMessage.Read);
        registry.Register(GameServerOpcode.SetInventory, (opcode, reader) => GameServerSetInventoryMessage.Read(opcode, reader, itemTypes));
        registry.Register(GameServerOpcode.DeleteInventory, GameServerDeleteInventoryMessage.Read);
    }
}
