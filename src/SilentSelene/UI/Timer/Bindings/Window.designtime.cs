#if DEBUG

using System;
using SilentSelene.Core;
using SilentSelene.Properties;

namespace SilentSelene.UI.Timer.Bindings;

public class DesignTimeWindow : Window
{
    public DesignTimeWindow()
        : base(INotifier.Default, UserSettings.Default) { }
}

#endif
