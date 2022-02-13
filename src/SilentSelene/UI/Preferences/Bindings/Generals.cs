using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Livet;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SilentSelene.Core;
using SilentSelene.Properties;
using SilentSelene.Utils;

namespace SilentSelene.UI.Preferences.Bindings;

public class Generals : ViewModel
{
    public IReactiveProperty<bool> ShowInTaskbar { get; }

    public IReactiveProperty<bool> TopMost { get; }

    public IReactiveProperty<bool> NotifyOverflow { get; }

    public IReadOnlyReactiveProperty<string> NotifyOverFlowLabel { get; }

    public IReactiveProperty<string> OverflowResin { get; }

    public Generals()
    {
        this.ShowInTaskbar = UserSettings.Default
            .ToReactivePropertyAsSynchronized(x => x.ShowInTaskbar);

        this.TopMost = UserSettings.Default
            .ToReactivePropertyAsSynchronized(x => x.TopMost);

        this.NotifyOverflow = UserSettings.Default
            .ToReactivePropertyAsSynchronized(x => x.NotifyOverflow);

        this.NotifyOverFlowLabel = this.NotifyOverflow
            .Select(x => x ? "オン" : "オフ")
            .ToReadOnlyReactiveProperty("");

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
