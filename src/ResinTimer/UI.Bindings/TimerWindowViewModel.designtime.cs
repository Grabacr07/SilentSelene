#if DEBUG

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ResinTimer.UI.Bindings
{
    public class DesignTimeTimerWindowViewModel : TimerWindowViewModel
    {
        [UsedImplicitly]
        public DesignTimeTimerWindowViewModel()
            : this(new Timer())
        {
        }

        public DesignTimeTimerWindowViewModel(Timer timer)
            : base(timer)
        {
            timer.Reset(new Random().Next(timer.MinResin.Value, timer.MaxResin.Value), false);
        }
    }
}

#endif
