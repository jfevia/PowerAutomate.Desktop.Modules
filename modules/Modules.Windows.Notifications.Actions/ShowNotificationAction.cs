// --------------------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// --------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK;
using Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK.Attributes;
using PowerAutomate.Desktop.Modules.Actions.Shared;

namespace PowerAutomate.Desktop.Modules.Windows.Notifications.Actions;

[Action(Id = "ShowNotification")]
[Throws(ErrorCodes.Unknown)]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "PowerAutomate.Desktop.Module.Action")]
public class ShowNotificationAction : ActionBase
{
    [InputArgument(Order = 1)]
    public string Text { get; set; } = null!;

    public override void Execute(ActionContext context)
    {
        Debugger.Launch();

        try
        {
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(toastXml.GetXml());

            var textElements = xmlDoc.GetElementsByTagName("text");
            textElements[0].AppendChild(xmlDoc.CreateTextNode(Text));

            var toast = new ToastNotification(xmlDoc)
            {
                ExpirationTime = DateTimeOffset.Now.AddMinutes(1)
            };

            var notifier = ToastNotificationManager.CreateToastNotifier("Microsoft.PowerAutomateDesktop.Apps");
            notifier.Show(toast);
        }
        catch (Exception ex)
        {
            throw new ActionException(ErrorCodes.Unknown, ex.Message, ex);
        }
    }
}