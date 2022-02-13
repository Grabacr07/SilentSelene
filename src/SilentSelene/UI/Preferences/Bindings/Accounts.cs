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

    public Accounts()
    {
        this.UseRealtimeNoteApi = UserSettings.Default
            .ToReactivePropertyAsSynchronized(x => x.UseRealtimeNoteApi);

        this.UseRealtimeNoteApiLabel = this.UseRealtimeNoteApi
            .Select(x => x ? "オン" : "オフ")
            .ToReadOnlyReactiveProperty("");

        this.Uid = UserSettings.Default
            .ToReactivePropertyAsSynchronized(x => x.uid);

        this.Ltuid = UserSettings.Default
            .ToReactivePropertyAsSynchronized(x => x.ltuid);

        this.Ltoken = UserSettings.Default
            .ToReactivePropertyAsSynchronized(x => x.ltoken);
    }
}
