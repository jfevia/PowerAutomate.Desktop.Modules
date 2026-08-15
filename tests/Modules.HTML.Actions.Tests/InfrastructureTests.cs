// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;

namespace PowerAutomate.Desktop.Modules.HTML.Actions.Tests;

[TestFixture]
public class InfrastructureTests
{
    [Test]
    public void Context_RequiresMarkupService()
    {
        Assert.Throws<ArgumentNullException>(() => new HtmlActionsContext(null!));
    }

    [Test]
    public void CreateDefault_WiresMarkupService()
    {
        Assert.That(HtmlActionsContext.CreateDefault().MarkupService, Is.TypeOf<HtmlMarkupService>());
    }

    [Test]
    public void Action_WithoutAContext_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ConvertListToHtmlListAction(null!));
    }
}
