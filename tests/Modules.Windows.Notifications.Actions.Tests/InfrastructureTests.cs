// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.Notifications.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.Windows.Notifications.Actions.Tests;

[TestFixture]
public class InfrastructureTests
{
    [Test]
    public void Context_RequiresNotificationService()
    {
        Assert.Throws<ArgumentNullException>(() => new NotificationsContext(null!, new FakeClock()));
    }

    [Test]
    public void Context_RequiresClock()
    {
        Assert.Throws<ArgumentNullException>(() => new NotificationsContext(new FakeNotificationService(), null!));
    }

    [Test]
    public void CreateDefault_WiresCollaborators()
    {
        var context = NotificationsContext.CreateDefault();

        Assert.That(context.NotificationService, Is.Not.Null);
        Assert.That(context.Clock, Is.Not.Null);
    }

    [Test]
    public void Action_WithoutAContext_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ShowNotificationAction(null!));
    }

    [Test]
    public void NotificationRequest_StoresValues()
    {
        var expiration = DateTimeOffset.Now;
        var request = new NotificationRequest("text", "app", expiration);

        Assert.That(request.Text, Is.EqualTo("text"));
        Assert.That(request.AppUserModelId, Is.EqualTo("app"));
        Assert.That(request.ExpirationTime, Is.EqualTo(expiration));
    }
}
