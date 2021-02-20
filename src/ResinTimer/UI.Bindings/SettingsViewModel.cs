using System;
using System.Collections.Generic;
using Livet;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ResinTimer.Functions;
using ResinTimer.Properties;

namespace ResinTimer.UI.Bindings
{
    public class SettingsViewModel : ViewModel
    {
        public IReactiveProperty<bool> ShowInTaskbar { get; }

        public IReactiveProperty<bool> TopMost { get; }

        public IReactiveProperty<bool> NotifyOverflow { get; }

        public IReactiveProperty<string> OverflowResin { get; }

        public SettingsViewModel()
        {
            this.ShowInTaskbar = UserSettings.Default
                .ToReactivePropertyAsSynchronized(x => x.ShowInTaskbar);

            this.TopMost = UserSettings.Default
                .ToReactivePropertyAsSynchronized(x => x.TopMost);

            this.NotifyOverflow = UserSettings.Default
                .ToReactivePropertyAsSynchronized(x => x.NotifyOverflow);

            this.OverflowResin = UserSettings.Default
                .ToReactivePropertyAsSynchronized(
                    x => x.OverflowResin,
                    x => x.ToString(),
                    x => int.TryParse(x, out var i)
                        ? i.EnsureRange(UserSettings.Default.MinResin, UserSettings.Default.MaxResin)
                        : UserSettings.Default.MaxResin);
        }
    }
}
