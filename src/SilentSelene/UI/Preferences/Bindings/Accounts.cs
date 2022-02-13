using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Livet;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SilentSelene.Properties;

namespace SilentSelene.UI.Preferences.Bindings;

public class Accounts : ViewModel
{
    public IReactiveProperty<bool> UseRealtimeNoteApi { get; }

    public IReadOnlyReactiveProperty<string> UseRealtimeNoteApiLabel { get; }

    public IReactiveProperty<string> Uid { get; }

    public IReactiveProperty<string> Ltuid { get; }

    public IReactiveProperty<string> Ltoken { get; }

    internal Accounts(UserSettings settings)
    {
        this.UseRealtimeNoteApi = settings
            .ToReactivePropertyAsSynchronized(x => x.UseRealtimeNoteApi)
            .AddTo(this.CompositeDisposable);

        this.UseRealtimeNoteApiLabel = this.UseRealtimeNoteApi
            .Select(x => x ? "オン" : "オフ")
            .ToReadOnlyReactiveProperty("");

        this.Uid = settings
            .ToReactivePropertyAsSynchronized(x => x.uid)
            .AddTo(this.CompositeDisposable);

        this.Ltuid = settings
            .ToReactivePropertyAsSynchronized(x => x.ltuid)
            .AddTo(this.CompositeDisposable);

        this.Ltoken = settings
            .ToReactivePropertyAsSynchronized(x => x.ltoken)
            .AddTo(this.CompositeDisposable);
    }
}
