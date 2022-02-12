#if DEBUG

using System;
using SilentSelene.Core;

namespace SilentSelene.UI.Timer.Bindings;

public partial class DesignTimeAuto : Auto
{
    public DesignTimeAuto()
        : base(new RealtimeNote())
    {
        this.UpdateAuthInfo();
    }

    partial void UpdateAuthInfo();
}

#endif
