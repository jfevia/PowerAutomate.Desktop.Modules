// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Editable;
using PowerAutomate.Desktop.OpenTibia.Protocol.Opcodes;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Editable;

[TestFixture]
public class EditableMessageReadersTests
{
    [Test]
    public void RegisterTo_RegistersEveryEditableOpcode()
    {
        var registry = new GameServerMessageRegistry();

        EditableMessageReaders.RegisterTo(registry);

        Assert.Multiple(() =>
        {
            Assert.That(registry.IsRegistered(GameServerOpcode.EditText), Is.True);
            Assert.That(registry.IsRegistered(GameServerOpcode.EditList), Is.True);
        });
    }

    [Test]
    public void RegisterTo_WithNullRegistry_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => EditableMessageReaders.RegisterTo(null!));
    }
}
