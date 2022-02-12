#if DEBUG

using System;
using SilentSelene.Core;

namespace SilentSelene.UI.Timer.Bindings;

public class DesignTimeManual : Manual
{
    public DesignTimeManual()
        : this(new ResinTimer())
    {
    }

    public DesignTimeManual(ResinTimer timer)
        : base(timer)
    {
        timer.Reset(new Random().Next(timer.MinResin.Value, timer.MaxResin.Value));
    }
}

#endif
