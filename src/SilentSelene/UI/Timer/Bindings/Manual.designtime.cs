#if DEBUG

using System;
using JetBrains.Annotations;
using SilentSelene.Core;
using SilentSelene.Properties;

namespace SilentSelene.UI.Timer.Bindings;

public class DesignTimeManual : Manual
{
    [UsedImplicitly]
    public DesignTimeManual()
        : this(new ResinTimer(INotifier.Default, UserSettings.Default)) { }

    public DesignTimeManual(ResinTimer timer)
        : base(timer)
    {
        timer.Reset(new Random().Next(timer.MinResin.Value, timer.MaxResin.Value));
    }
}

#endif
