using System;

namespace ResinTimer.Notification
{
    public interface INotifier
    {
        public void Notify(string title, string body);

        public static INotifier Default
            => new DesktopToast();
    }
}
