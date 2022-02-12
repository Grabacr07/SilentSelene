#if DEBUG

using System.ComponentModel;
using System.Windows;
using SilentSelene.Core;

namespace SilentSelene;

public class DebugFeatures
{
    public static void Notify()
    {
        new DesktopToast().Notify("test", "toastだょ");
    }
}

#endif
