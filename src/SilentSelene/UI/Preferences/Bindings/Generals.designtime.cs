#if DEBUG

using System;
using SilentSelene.Properties;

namespace SilentSelene.UI.Preferences.Bindings;

public class DesignTimeGenerals : Generals
{
    public DesignTimeGenerals()
        : base(UserSettings.Default) { }
}

#endif
