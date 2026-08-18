// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests;

[TestFixture]
public class ProtocolExceptionTests
{
    [Test]
    public void Constructor_WithMessage_KeepsMessage()
    {
        var exception = new ProtocolException("broken");

        Assert.That(exception.Message, Is.EqualTo("broken"));
    }

    [Test]
    public void Constructor_WithInnerException_KeepsBoth()
    {
        var inner = new InvalidOperationException("cause");

        var exception = new ProtocolException("broken", inner);

        Assert.Multiple(() =>
        {
            Assert.That(exception.Message, Is.EqualTo("broken"));
            Assert.That(exception.InnerException, Is.SameAs(inner));
        });
    }
}
