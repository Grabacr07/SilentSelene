#if DEBUG

using System;
using SilentSelene.Core;
using SilentSelene.Properties;

namespace SilentSelene.UI.Timer.Bindings;

public partial class DesignTimeAuto : Auto
{
    public DesignTimeAuto()
        : base(new RealtimeNote(INotifier.Default, UserSettings.Default))
    {
        this.UpdateAuthInfo();
    }

    partial void UpdateAuthInfo();
}

#endif
