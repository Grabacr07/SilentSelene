﻿using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ResinTimer.Functions;
using ResinTimer.Notification;
using ResinTimer.Properties;
using SystemTimer = System.Timers.Timer;

namespace ResinTimer
{
    public class Timer : IDisposable
    {
        private const int MinutesOfResin = 8;

        private readonly INotifier _notifier;
        private readonly ReactiveProperty<DateTimeOffset> _overflowingTime = new ReactiveProperty<DateTimeOffset>();
        private readonly ReactiveProperty<TimeSpan> _remainingTime = new ReactiveProperty<TimeSpan>();
        private readonly ReactiveProperty<int> _currentResin = new ReactiveProperty<int>();
        private readonly SystemTimer _systemTimer = new SystemTimer(500);
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public IReadOnlyReactiveProperty<int> MaxResin { get; }

        public IReadOnlyReactiveProperty<int> MinResin { get; }

        public IReadOnlyReactiveProperty<int> CurrentResin
            => this._currentResin;

        public IReadOnlyReactiveProperty<DateTimeOffset> OverflowingTime
            => this._overflowingTime;

        public IReadOnlyReactiveProperty<TimeSpan> RemainingTime
            => this._remainingTime;

        public IReadOnlyReactiveProperty<bool> IsOverflow { get; }

        public Timer()
            : this(INotifier.Default)
        {
        }

        public Timer(INotifier notifier)
        {
            this._notifier = notifier;

            this.MaxResin = UserSettings.Default.ToReactivePropertyAsSynchronized(x => x.MaxResin);
            this.MinResin = UserSettings.Default.ToReactivePropertyAsSynchronized(x => x.MinResin);

            this.IsOverflow = this._currentResin
                .CombineLatest(
                    UserSettings.Default
                        .ToReactivePropertyAsSynchronized(x => x.OverflowResin)
                        .AddTo(this._compositeDisposable))
                .Select<(int current, int overflow), bool>(x => x.current >= x.overflow)
                .ToReadOnlyReactiveProperty();

            this.IsOverflow
                .Where(x => x)
                .Subscribe(_ => this.NotifyOverflow());

            var latest = UserSettings.Default.LatestOverflowTime;
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
                .Where(x => MetroTrilithon.DebugFeatures.IsInDesignMode == false)
#endif
                .Skip(1)
                .Subscribe(x =>
                {
                    UserSettings.Default.LatestOverflowTime = x;
                    UserSettings.Default.Save();
                });

            this._systemTimer.Elapsed += (sender, args) => this.Tick(new DateTimeOffset(args.SignalTime));
            this._systemTimer.Start();
        }

        public void Reset(int resin)
        {
            var time = DateTimeOffset.Now.AddMinutes((this.MaxResin.Value - resin) * MinutesOfResin);
            this.EnsureOverflowingRange(time);
        }

        public void Increase(int resin)
        {
            var targetTime = DateTimeUtil.Later(this._overflowingTime.Value, DateTimeOffset.Now);
            var time = targetTime.Subtract(TimeSpan.FromMinutes(resin * MinutesOfResin));

            this.EnsureOverflowingRange(time);
        }

        private void EnsureOverflowingRange(DateTimeOffset time)
        {
            var max = DateTimeOffset.Now.AddMinutes(this.MaxResin.Value * MinutesOfResin);
            var min = DateTimeOffset.Now.AddMinutes(this.MinResin.Value * MinutesOfResin);

            this._overflowingTime.Value = time.EnsureRange(min, max);
        }

        private void Tick(DateTimeOffset signalTime)
        {
            var remaining = this._overflowingTime.Value.Subtract(signalTime);

            this._remainingTime.Value = remaining;
            this._currentResin.Value = (this.MaxResin.Value - (int)Math.Ceiling(remaining.TotalMinutes / MinutesOfResin)).EnsureRange(this.MinResin.Value, this.MaxResin.Value);
        }

        private void NotifyOverflow()
        {
            if (UserSettings.Default.NotifyOverflow)
            {
                this._notifier.Notify(
                    "天然樹脂が回復しました",
                    $"現在の天然樹脂は {this._currentResin.Value} です。早く使ってね！");
            }
        }

        public void Dispose()
        {
            this._systemTimer.Dispose();
        }
    }
}
