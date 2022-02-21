using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using GenshinInfo.Managers;
using MetroTrilithon.Threading.Tasks;
using Reactive.Bindings;
using SilentSelene.Properties;
using SilentSelene.Utils;

namespace SilentSelene.Core;

public enum RealtimeNoteStatus
{
    Active,
    AuthError,
    NoteError,
}

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
    private readonly ReactiveProperty<RealtimeNoteStatus> _status = new();
    private GenshinInfoManager _manager;
    private DateTimeOffset _nextReload;

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

    public IReadOnlyReactiveProperty<RealtimeNoteStatus> Status
        => this._status;
    
    internal RealtimeNote(INotifier notifier, UserSettings settings)
        : base(notifier, settings)
    {
        this._settings = settings;
        this._manager = new GenshinInfoManager(settings.uid, settings.ltuid, settings.ltoken);
    }

    public Task<bool> Check()
        => this.CheckCore(true);

    public async Task<bool> CheckCore(bool updateStatus)
    {
        var success = await this._manager.CheckLogin();
        if (updateStatus)
        {
            this._status.Value = success switch
            {
                true when this._status.Value != RealtimeNoteStatus.Active => RealtimeNoteStatus.Active,
                false when this._status.Value != RealtimeNoteStatus.AuthError => RealtimeNoteStatus.AuthError,
                _ => this._status.Value
            };
        }

        return success;
    }

    public async Task<bool> Reload()
    {
        var check = await this.CheckCore(false);
        if (check == false)
        {
            this._status.Value = RealtimeNoteStatus.AuthError;
            this._nextReload = DateTimeUtil.Earlier(this._nextReload, DateTimeOffset.Now.AddSeconds(30));
            return false;
        }

        var note = await this._manager.GetRealTimeNotes();
        if (note == null)
        {
            this._status.Value = RealtimeNoteStatus.NoteError;
            this._nextReload = DateTimeUtil.Earlier(this._nextReload, DateTimeOffset.Now.AddSeconds(30));
            return false;
        }

        this.EnsureOverflowingRange(DateTimeOffset.Now.Add(note.ResinRecoveryTime));
        this._coinOverflowingTime.Value = DateTimeOffset.Now.Add(note.HomeCoinRecoveryTime);
        this._currentCoin.Value = note.CurrentHomeCoin;
        this._maxCoin.Value = note.MaxHomeCoin;
        this._finishedTask.Value = note.FinishedTaskNum;
        this._totalTask.Value = note.TotalTaskNum;
        this._isTaskRewardReceived.Value = note.IsExtraTaskRewardReceived;

        this._status.Value = RealtimeNoteStatus.Active;
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

        if (signalTime < this._nextReload) return;

        var dateline = new DateTimeOffset(signalTime.Year, signalTime.Month, signalTime.Day, 5, 0, 0, signalTime.Offset);
        this._nextReload = signalTime >= dateline
            ? signalTime.Add(this._settings.ApiRequestInterval)
            : DateTimeUtil.Earlier(dateline, signalTime.Add(this._settings.ApiRequestInterval));

        this.Reload().Forget();
    }
}
