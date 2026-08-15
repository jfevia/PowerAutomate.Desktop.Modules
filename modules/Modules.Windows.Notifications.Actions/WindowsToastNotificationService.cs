// ---------------------------------------------------
// Copyright (c) Jesus Fernandez. All Rights Reserved.
// ---------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace PowerAutomate.Desktop.Modules.Windows.Notifications.Actions;

// Pure toast adapter; every member forwards to Windows notification APIs.
[ExcludeFromCodeCoverage]
internal sealed class WindowsToastNotificationService : INotificationService
{
    public void Show(NotificationRequest notification)
    {
        var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(toastXml.GetXml());
        xmlDoc.GetElementsByTagName("text")[0].AppendChild(xmlDoc.CreateTextNode(notification.Text));
        var toast = new ToastNotification(xmlDoc) { ExpirationTime = notification.ExpirationTime };
        ToastNotificationManager.CreateToastNotifier(notification.AppUserModelId).Show(toast);
    }
}
