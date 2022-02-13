using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using SilentSelene.Core;
using SilentSelene.Properties;

namespace SilentSelene;

partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        WPFUI.Theme.Watcher.Start(true, true);

        var window = new UI.Timer.Window()
        {
            DataContext = new UI.Timer.Bindings.Window(INotifier.Default, UserSettings.Default),
        };
        window.Show();
    }
}
