using System;

namespace SilentSelene.Core;

public interface INotifier
{
    public void Notify(string title, string body);

    public static INotifier Default { get; }
        = new DesktopToast();
}
