#if DEBUG

using System;
using SilentSelene.Properties;

namespace SilentSelene.UI.Preferences.Bindings;

public class DesignTimeWindow : Window
{
    public DesignTimeWindow()
        : base(UserSettings.Default) { }
}

#endif
