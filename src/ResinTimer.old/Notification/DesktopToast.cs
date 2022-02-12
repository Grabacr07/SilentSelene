﻿using System;
using System.Collections.Generic;
using Windows.UI.Notifications;
using ResinTimer.Properties;

namespace ResinTimer.Notification
{
    public class DesktopToast : INotifier
    {
        public void Notify(string title, string body)
        {
            var xml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            var texts = xml.GetElementsByTagName("text");
            texts[0].AppendChild(xml.CreateTextNode(title));
            texts[1].AppendChild(xml.CreateTextNode(body));

            var toast = new ToastNotification(xml);
            ToastNotificationManager.CreateToastNotifier(AssemblyInfo.Product).Show(toast);
        }
    }
}
