// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using NUnit.Framework;
using PowerAutomate.Desktop.Modules.Windows.Notifications.Actions.Tests.Fakes;

namespace PowerAutomate.Desktop.Modules.Windows.Notifications.Actions.Tests.Actions;

[TestFixture]
public class ShowNotificationActionTests
{
    [Test]
    public void Execute_CreatesNotificationRequest()
    {
        var service = new FakeNotificationService();
        var clock = new FakeClock();
        var action = new ShowNotificationAction(new NotificationsContext(service, clock)) { Text = "hello" };

        action.Execute(new ActionContext());

        Assert.That(service.LastNotification, Is.Not.Null);
        Assert.That(service.LastNotification!.Text, Is.EqualTo("hello"));
        Assert.That(service.LastNotification.AppUserModelId, Is.EqualTo("Microsoft.PowerAutomateDesktop.Apps"));
        Assert.That(service.LastNotification.ExpirationTime, Is.EqualTo(clock.Now.AddMinutes(1)));
    }

    [Test]
    public void Execute_WhenNotificationServiceFails_ThrowsUnknownError()
    {
        var service = new FakeNotificationService { ExceptionToThrow = new InvalidOperationException("boom") };
        var action = new ShowNotificationAction(new NotificationsContext(service, new FakeClock())) { Text = "hello" };

        var exception = Assert.Throws<ActionException>(() => action.Execute(new ActionContext()))!;

        Assert.That(exception.Name, Is.EqualTo("UnknownError"));
    }

    [Test]
    public void DefaultConstructor_UsesProductionWiring()
    {
        var action = new ShowNotificationAction();

        Assert.That(action.Text, Is.Null);
    }
}
