// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using NUnit.Framework;
using PowerAutomate.Desktop.OpenTibia.Protocol.Messages.Map;

namespace PowerAutomate.Desktop.OpenTibia.Protocol.Tests.Messages.Map;

[TestFixture]
public class DefaultItemTypeProviderTests
{
    [Test]
    public void Predicates_ForAnyItemId_AllReturnFalse()
    {
        var provider = new DefaultItemTypeProvider();

        Assert.Multiple(() =>
        {
            Assert.That(provider.IsStackable(100), Is.False);
            Assert.That(provider.IsFluidContainer(200), Is.False);
            Assert.That(provider.IsSplash(300), Is.False);
        });
    }
}
