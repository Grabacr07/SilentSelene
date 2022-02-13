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

    internal Generals(UserSettings settings)
    {
        this.ShowInTaskbar = settings
            .ToReactivePropertyAsSynchronized(x => x.ShowInTaskbar)
            .AddTo(this.CompositeDisposable);

        this.TopMost = settings
            .ToReactivePropertyAsSynchronized(x => x.TopMost)
            .AddTo(this.CompositeDisposable);

        this.NotifyOverflow = settings
            .ToReactivePropertyAsSynchronized(x => x.NotifyOverflow)
            .AddTo(this.CompositeDisposable);

        this.NotifyOverFlowLabel = this.NotifyOverflow
            .Select(x => x ? "オン" : "オフ")
            .ToReadOnlyReactiveProperty("");

        this.OverflowResin = settings
            .ToReactivePropertyAsSynchronized(
                x => x.OverflowResin,
                x => x.ToString(),
                x => int.TryParse(x, out var i)
                    ? i.EnsureRange(settings.MinResin, settings.MaxResin)
                    : settings.MaxResin)
            .AddTo(this.CompositeDisposable);
    }

    [UsedImplicitly]
    public void TestNotification()
    {
        INotifier.Default.Notify("テスト", $"これは {AssemblyInfo.Product} の通知テストです。");
    }
}
