#if DEBUG

using System;
using SilentSelene.Properties;

namespace SilentSelene.UI.Preferences.Bindings;

public class DesignTimeAccounts : Accounts
{
    public DesignTimeAccounts()
        : base(UserSettings.Default) { }
}

#endif
