using System;
using System.Collections.Generic;
using Livet;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ResinTimer.Properties;

namespace ResinTimer.UI.Bindings
{
    public class SettingsViewModel : ViewModel
    {
        public IReactiveProperty<bool> ShowInTaskbar { get; }

        public IReactiveProperty<bool> TopMost { get; }

        public SettingsViewModel()
        {
            this.ShowInTaskbar = UserSettings.Default
                .ToReactivePropertyAsSynchronized(x => x.ShowInTaskbar);

            this.TopMost = UserSettings.Default
                .ToReactivePropertyAsSynchronized(x => x.TopMost);
        }
    }
}
