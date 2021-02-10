using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using ResinTimer.Functions;
using ResinTimer.Properties;
using SystemTimer = System.Timers.Timer;

namespace ResinTimer
{
    public class Timer : IDisposable
    {
        private const int MinutesOfResin = 8;

        private readonly ReactiveProperty<DateTimeOffset> _overflowingTime = new ReactiveProperty<DateTimeOffset>();
        private readonly ReactiveProperty<TimeSpan> _remainingTime = new ReactiveProperty<TimeSpan>();
        private readonly ReactiveProperty<int> _currentResin = new ReactiveProperty<int>();
        private readonly SystemTimer _systemTimer = new SystemTimer(1000);

        public IReadOnlyReactiveProperty<int> MaxResin { get; }

        public IReadOnlyReactiveProperty<int> MinResin { get; }

        public IReadOnlyReactiveProperty<int> CurrentResin
            => this._currentResin;

        public IReadOnlyReactiveProperty<DateTimeOffset> OverflowingTime
            => this._overflowingTime;

        public IReadOnlyReactiveProperty<TimeSpan> RemainingTime
            => this._remainingTime;

        public Timer()
        {
            this.MaxResin = UserSettings.Default.ToReactivePropertyAsSynchronized(x => x.MaxResin);
            this.MinResin = UserSettings.Default.ToReactivePropertyAsSynchronized(x => x.MinResin);
            
            this._systemTimer.Elapsed += (sender, args) => this.Tick(new DateTimeOffset(args.SignalTime));
            this._systemTimer.Start();

            this.Reset(this.MaxResin.Value);
        }

        public void Reset(int resin)
        {
            var minutes = (this.MaxResin.Value - resin).EnsureRange(this.MinResin.Value, this.MaxResin.Value) * MinutesOfResin;
            this._overflowingTime.Value = DateTimeOffset.Now.AddMinutes(minutes);
        }

        private void Tick(DateTimeOffset signalTime)
        {
            var remaining = this._overflowingTime.Value.Subtract(signalTime);

            this._remainingTime.Value = remaining;
            this._currentResin.Value = (this.MaxResin.Value - (int)Math.Ceiling(remaining.TotalMinutes / MinutesOfResin)).EnsureRange(this.MinResin.Value, this.MaxResin.Value);
        }

        public void Dispose()
        {
            this._systemTimer.Dispose();
        }
    }
}
