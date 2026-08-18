// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;

namespace PowerAutomate.Desktop.Modules.Windows.Notifications.Actions;

[Action(Id = "ShowNotification")]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class ShowNotificationAction : NotificationsActionBase
{
    private const string AppUserModelId = "Microsoft.PowerAutomateDesktop.Apps";

    public ShowNotificationAction()
    {
    }

    internal ShowNotificationAction(NotificationsContext context) : base(context)
    {
    }

    [InputArgument(Order = 1)]
    public string Text { get; set; } = null!;

    protected override void Run(ActionContext context) => Context.NotificationService.Show(new NotificationRequest(Text, AppUserModelId, Context.Clock.Now.AddMinutes(1)));
}
