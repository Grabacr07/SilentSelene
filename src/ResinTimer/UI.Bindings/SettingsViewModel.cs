using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Livet;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ResinTimer.Functions;
using ResinTimer.Notification;
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

        [UsedImplicitly]
        public void TestNotification()
        {
            INotifier.Default.Notify("テスト", $"これは {AssemblyInfo.Product} の通知テストです。");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            UserSettings.Default.Save();
        }
    }
}
