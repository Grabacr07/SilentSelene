using System;
using System.Collections.Generic;
using Microsoft.Toolkit.Uwp.Notifications;

namespace SilentSelene.Core
{
    public class DesktopToast : INotifier
    {
        public void Notify(string title, string body)
        {
            new ToastContentBuilder()
                .AddText(title, AdaptiveTextStyle.Title)
                .AddText(body, AdaptiveTextStyle.Body)
                .Show();
        }
    }
}
