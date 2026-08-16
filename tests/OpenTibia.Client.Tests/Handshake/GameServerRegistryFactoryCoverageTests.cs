// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Client.Handshake;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Client.Tests.Handshake;

[TestFixture]
public class GameServerRegistryFactoryCoverageTests
{
    /// <summary>
    /// Guards against protocol gaps like the live 0x79 failure this suite fixed: every opcode must decode.
    /// </summary>
    [Test]
    public void CreateDefault_LeavesNoOpcodeUnregistered()
    {
        var registry = GameServerRegistryFactory.CreateDefault();

        Assert.Multiple(() =>
        {
            foreach (GameServerOpcode opcode in Enum.GetValues(typeof(GameServerOpcode)))
            {
                if (opcode == GameServerOpcode.None)
                {
                    continue;
                }

                Assert.That(registry.IsRegistered(opcode), Is.True, $"{opcode} (0x{(byte)opcode:X2}) has no reader.");
            }
        });
    }
}
