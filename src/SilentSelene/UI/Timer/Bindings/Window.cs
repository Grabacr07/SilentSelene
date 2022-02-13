using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Livet.Messaging;
using Livet.Messaging.Windows;
using MetroTrilithon.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SilentSelene.Properties;

namespace SilentSelene.UI.Timer.Bindings;

public class Window : WindowBase
{
    public const string OpenPreferencesMessageKey = nameof(OpenPreferencesMessageKey);

    private readonly ReactiveProperty<object> _content = new();

    public IReadOnlyReactiveProperty<object> Content
        => this._content;

    public IReadOnlyReactiveProperty<bool> ShowInTaskbar { get; }

    public IReadOnlyReactiveProperty<bool> TopMost { get; }

    public Window()
    {
        this._content.Value = new Manual();

        this.ShowInTaskbar = UserSettings.Default
            .ToReactivePropertyAsSynchronized(x => x.ShowInTaskbar);

        this.TopMost = UserSettings.Default
            .ToReactivePropertyAsSynchronized(x => x.TopMost);
    }

    [UsedImplicitly]
    public void OpenPreferences()
    {
        var context = new Preferences.Bindings.Window();
        var message = new TransitionMessage(context, OpenPreferencesMessageKey);
        this.Messenger.Raise(message);
    }

    [UsedImplicitly]
    public void Minimize()
        => this.SendWindowAction(WindowAction.Minimize);
}
