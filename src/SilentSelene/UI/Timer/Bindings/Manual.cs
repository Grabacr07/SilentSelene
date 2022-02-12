using System;
using System.Collections.Generic;
using System.Linq;
using Livet;
using Livet.Messaging;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SilentSelene.Core;
using SilentSelene.Properties;

namespace SilentSelene.UI.Timer.Bindings;

public class Manual : ViewModel
{
    public const string ShowOptionWindowMessageKey = nameof(ShowOptionWindowMessageKey);

    public ResinTimer ResinTimer { get; }

    public IReactiveProperty<string> NewResin { get; }

    public IReadOnlyReactiveProperty<bool> ShowInTaskbar { get; }

    public IReadOnlyReactiveProperty<bool> TopMost { get; }

    public Manual()
        : this(new ResinTimer())
    {
    }

    public Manual(ResinTimer resinTimer)
    {
        this.ResinTimer = resinTimer;
        this.NewResin = new ReactiveProperty<string>();

        //ToDo
        //resinTimer.IsOverflow
        //    .Subscribe(x => ThemeService.Current.ChangeAccent(x ? Accent.Orange : Accent.Blue));

        this.ShowInTaskbar = UserSettings.Default
            .ToReactivePropertyAsSynchronized(x => x.ShowInTaskbar);

        this.TopMost = UserSettings.Default
            .ToReactivePropertyAsSynchronized(x => x.TopMost);
    }
        
    public void Update()
    {
        if (int.TryParse(this.NewResin.Value, out var resin))
        {
            this.ResinTimer.Reset(resin);
        }

        this.NewResin.Value = "";
    }
        
    public void Settings()
    {
        //var binding = new MainWindowViewModel();
        //var message = new TransitionMessage(binding, ShowOptionWindowMessageKey);
        //this.Messenger.Raise(message);
    }
        
    public void Increase(string value)
    {
        if (int.TryParse(value, out var i)) this.ResinTimer.Increase(i);
    }
}
