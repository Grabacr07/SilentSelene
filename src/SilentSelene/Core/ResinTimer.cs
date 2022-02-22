using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SilentSelene.Properties;
using SilentSelene.Utils;
using SystemTimer = System.Timers.Timer;

namespace SilentSelene.Core;

public class ResinTimer : IDisposable
{
    private const int _minutesOfResin = 8;

    private readonly INotifier _notifier;
    private readonly UserSettings _settings;
    private readonly ReactiveProperty<DateTimeOffset> _overflowingTime = new();
    private readonly ReactiveProperty<TimeSpan> _remainingTime = new();
    private readonly ReactiveProperty<int> _currentResin = new();
    private readonly SystemTimer _systemTimer = new(500);

    protected CompositeDisposable CompositeDisposable { get; } = new();

    public IReadOnlyReactiveProperty<int> MaxResin { get; }

    public IReadOnlyReactiveProperty<int> MinResin { get; }

    public IReadOnlyReactiveProperty<int> CurrentResin
        => this._currentResin;

    public IReadOnlyReactiveProperty<DateTimeOffset> OverflowingTime
        => this._overflowingTime;

    public IReadOnlyReactiveProperty<TimeSpan> RemainingTime
        => this._remainingTime;

    public IReadOnlyReactiveProperty<bool> IsOverflow { get; }

    internal ResinTimer(INotifier notifier, UserSettings settings)
    {
        this._notifier = notifier;
        this._settings = settings;

        this.MaxResin = settings
            .ToReactivePropertyAsSynchronized(x => x.MaxResin)
            .AddTo(this.CompositeDisposable);
        this.MinResin = settings
            .ToReactivePropertyAsSynchronized(x => x.MinResin)
            .AddTo(this.CompositeDisposable);

        this.IsOverflow = this._currentResin
            .CombineLatest(
                settings
                    .ToReactivePropertyAsSynchronized(x => x.OverflowResin)
                    .AddTo(this.CompositeDisposable))
            .Select<(int current, int overflow), bool>(x => x.current >= x.overflow)
            .ToReadOnlyReactiveProperty();

        this.IsOverflow
            .Where(x => x)
            .Subscribe(_ => this.NotifyOverflow());

        var latest = settings.LatestOverflowTime;
        if (latest > DateTimeOffset.Now)
        {
            this._overflowingTime.Value = latest;
        }
        else
        {
            this.Reset(this.MinResin.Value);
        }

        this._overflowingTime
#if DEBUG
            .Where(_ => MetroTrilithon.DebugFeatures.IsInDesignMode == false)
#endif
            .Skip(1)
            .Subscribe(
                x =>
                {
                    settings.LatestOverflowTime = x;
                    settings.Save();
                });

        this._systemTimer.Elapsed += (_, args) => this.Tick(new DateTimeOffset(args.SignalTime));
        this._systemTimer.Start();
    }

    public void Reset(int resin)
    {
        var time = DateTimeOffset.Now.AddMinutes((this.MaxResin.Value - resin) * _minutesOfResin);
        this.EnsureOverflowingRange(time);
    }

    public void Increase(int resin)
    {
        var targetTime = DateTimeUtil.Later(this._overflowingTime.Value, DateTimeOffset.Now);
        var time = targetTime.Subtract(TimeSpan.FromMinutes(resin * _minutesOfResin));

        this.EnsureOverflowingRange(time);
    }

    protected void EnsureOverflowingRange(DateTimeOffset time)
    {
        var max = DateTimeOffset.Now.AddMinutes(this.MaxResin.Value * _minutesOfResin);
        var min = DateTimeOffset.Now.AddMinutes(this.MinResin.Value * _minutesOfResin);

        this._overflowingTime.Value = time.EnsureRange(min, max);
    }

    protected virtual void Tick(DateTimeOffset signalTime)
    {
        var remaining = this._overflowingTime.Value.Subtract(signalTime);

        this._remainingTime.Value = remaining.TotalSeconds > 0 ? remaining : TimeSpan.Zero;
        this._currentResin.Value = (this.MaxResin.Value - (int)Math.Ceiling(remaining.TotalMinutes / _minutesOfResin)).EnsureRange(this.MinResin.Value, this.MaxResin.Value);
    }

    private void NotifyOverflow()
    {
        if (this._settings.NotifyOverflow)
        {
            this._notifier.Notify(
                "天然樹脂が回復しました",
                $"現在の天然樹脂は {this._currentResin.Value} です。早く使ってね！");
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            this._systemTimer.Dispose();
            this.CompositeDisposable.Dispose();
        }
    }
}
