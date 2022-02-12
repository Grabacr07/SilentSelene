using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Livet.Messaging.Windows;
using MetroTrilithon.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SilentSelene.Core;
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

    public void OpenPreferences()
    {
    }

    public void Minimize()
        => this.SendWindowAction(WindowAction.Minimize);
}
