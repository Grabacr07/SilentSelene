using System;
using System.Collections.Generic;
using System.Linq;
using GenshinInfo.Managers;
using MetroTrilithon.Threading.Tasks;
using Reactive.Bindings;
using SilentSelene.Properties;
using SilentSelene.Utils;

namespace SilentSelene.Core;

public class RealtimeNote : ResinTimer
{
    private readonly UserSettings _settings;
    private readonly ReactiveProperty<DateTimeOffset> _coinOverflowingTime = new(DateTimeOffset.Now);
    private readonly ReactiveProperty<TimeSpan> _coinRemainingTime = new();
    private readonly ReactiveProperty<int> _currentCoin = new();
    private readonly ReactiveProperty<int> _maxCoin = new(2400);
    private readonly ReactiveProperty<int> _finishedTask = new();
    private readonly ReactiveProperty<int> _totalTask = new(4);
    private readonly ReactiveProperty<bool> _isTaskRewardReceived = new();
    private readonly ReactiveProperty<bool> _hasError = new();
    private readonly TimeSpan _interval = new(0, 15, 0);
    private GenshinInfoManager _manager;
    private DateTimeOffset _next;

    public IReadOnlyReactiveProperty<DateTimeOffset> CoinOverflowingTime
        => this._coinOverflowingTime;

    public IReactiveProperty<TimeSpan> CoinRemainingTime
        => this._coinRemainingTime;

    public IReadOnlyReactiveProperty<int> CurrentCoin
        => this._currentCoin;

    public IReadOnlyReactiveProperty<int> MaxCoin
        => this._maxCoin;

    public IReadOnlyReactiveProperty<int> FinishedTask
        => this._finishedTask;

    public IReadOnlyReactiveProperty<int> TotalTask
        => this._totalTask;

    public IReadOnlyReactiveProperty<bool> IsTaskRewardReceived
        => this._isTaskRewardReceived;

    public IReadOnlyReactiveProperty<bool> HasError
        => this._hasError;

    internal RealtimeNote(INotifier notifier, UserSettings settings)
        : base(notifier, settings)
    {
        this._settings = settings;
        this._manager = new GenshinInfoManager(settings.uid, settings.ltuid, settings.ltoken);
    }

    public async Task<bool> Check()
    {
        var success = await this._manager.CheckLogin();
        this._hasError.Value = success == false;
        return success;
    }

    public async Task<bool> Reload()
    {
        var check = await this.Check();
        if (check == false) return false;

        var note = await this._manager.GetRealTimeNotes();
        this.EnsureOverflowingRange(DateTimeOffset.Now.Add(note.ResinRecoveryTime));
        this._coinOverflowingTime.Value = DateTimeOffset.Now.Add(note.HomeCoinRecoveryTime);
        this._currentCoin.Value = note.CurrentHomeCoin;
        this._maxCoin.Value = note.MaxHomeCoin;
        this._finishedTask.Value = note.FinishedTaskNum;
        this._totalTask.Value = note.TotalTaskNum;
        this._isTaskRewardReceived.Value = note.IsExtraTaskRewardReceived;

        return true;
    }

    public Task<bool> UpdateAuthInfo()
        => this.UpdateAuthInfo(this._settings.uid, this._settings.ltuid, this._settings.ltoken);

    public Task<bool> UpdateAuthInfo(string uid, string ltuid, string ltoken)
    {
        this._manager = new GenshinInfoManager(uid, ltuid, ltoken);

        return this.Reload();
    }

    protected override void Tick(DateTimeOffset signalTime)
    {
        base.Tick(signalTime);

        this._coinRemainingTime.Value = this._coinOverflowingTime.Value.Subtract(signalTime);

        if (signalTime < this._next) return;

        var dateline = new DateTimeOffset(signalTime.Year, signalTime.Month, signalTime.Day, 5, 0, 0, signalTime.Offset);
        this._next = signalTime >= dateline
            ? signalTime.Add(this._interval)
            : DateTimeUtil.Earlier(dateline, signalTime.Add(this._interval));

        this.Reload().Forget();
    }
}
