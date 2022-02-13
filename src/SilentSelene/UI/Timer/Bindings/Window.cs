using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using JetBrains.Annotations;
using Livet.Messaging;
using Livet.Messaging.Windows;
using MetroTrilithon.Mvvm;
using MetroTrilithon.Threading.Tasks;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SilentSelene.Core;
using SilentSelene.Properties;

namespace SilentSelene.UI.Timer.Bindings;

public class Window : WindowBase
{
    public const string OpenPreferencesMessageKey = nameof(OpenPreferencesMessageKey);

    private readonly ReactiveProperty<object?> _content = new();
    private readonly SerialDisposable _serialDisposable = new();
    private readonly INotifier _notifier;
    private readonly UserSettings _settings;

    public IReadOnlyReactiveProperty<object?> Content
        => this._content;

    public IReadOnlyReactiveProperty<bool> ShowInTaskbar { get; }

    public IReadOnlyReactiveProperty<bool> TopMost { get; }

    internal Window(INotifier notifier, UserSettings settings)
    {
        this._notifier = notifier;
        this._settings = settings;
        this._content.Value = "waiting...";

        this.ShowInTaskbar = settings
            .ToReactivePropertyAsSynchronized(x => x.ShowInTaskbar)
            .AddTo(this.CompositeDisposable);

        this.TopMost = settings
            .ToReactivePropertyAsSynchronized(x => x.TopMost)
            .AddTo(this.CompositeDisposable);

        var useApi = settings
            .ToReactivePropertyAsSynchronized(x => x.UseRealtimeNoteApi)
            .AddTo(this.CompositeDisposable);
        var uid = settings
            .ToReactivePropertyAsSynchronized(x => x.uid)
            .AddTo(this.CompositeDisposable);
        var ltuid = settings
            .ToReactivePropertyAsSynchronized(x => x.ltuid)
            .AddTo(this.CompositeDisposable);
        var ltoken = settings
            .ToReactivePropertyAsSynchronized(x => x.ltoken)
            .AddTo(this.CompositeDisposable);

        useApi.CombineLatest(uid, ltuid, ltoken)
            .Subscribe(_ => this.UpdateContent().Forget());
    }

    public async Task UpdateContent()
    {
        if (this._settings.UseRealtimeNoteApi
            && string.IsNullOrEmpty(this._settings.uid) == false
            && string.IsNullOrEmpty(this._settings.ltuid) == false
            && string.IsNullOrEmpty(this._settings.ltoken) == false)
        {
            if (this._content.Value is Auto auto)
            {
                await auto.RealtimeNote.UpdateAuthInfo();
            }
            else
            {
                auto = new Auto(new RealtimeNote(this._notifier, this._settings));
                this._content.Value = auto;
                this._serialDisposable.Disposable = auto;
            }
        }
        else
        {
            if (this._content.Value is not Manual)
            {
                var timer = new ResinTimer(this._notifier, this._settings);
                this._content.Value = new Manual(timer);
                this._serialDisposable.Disposable = timer;
            }
        }
    }

    [UsedImplicitly]
    public void OpenPreferences()
    {
        var context = new Preferences.Bindings.Window(this._settings);
        var message = new TransitionMessage(context, OpenPreferencesMessageKey);
        this.Messenger.Raise(message);
    }

    [UsedImplicitly]
    public void Minimize()
        => this.SendWindowAction(WindowAction.Minimize);
}
