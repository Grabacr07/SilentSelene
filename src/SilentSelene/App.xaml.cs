using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MetroTrilithon.Threading.Tasks;
using MetroTrilithon.UI;
using SilentSelene.Core;
using SilentSelene.Properties;
using SilentSelene.Reporting;

namespace SilentSelene;

partial class App
{
    static App()
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            if (args.ExceptionObject is Exception exception) IReporter.Instance.ReportException(sender, exception);
        };
    }

    public App()
    {
        SettingsUtil.Update();

        UserSettings.Default.MaxResin = 200;
        UserSettings.Default.Save();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        TaskLog.Occurred += (sender, args) => IReporter.Instance.ReportException(sender ?? typeof(TaskLog), args.Exception);
        UIDispatcher.Instance = this.Dispatcher;
        
        this.DispatcherUnhandledException += (sender, args) =>
        {
            IReporter.Instance.ReportException(sender, args.Exception);
            args.Handled = true;
        };

        this.Break();

        var context = new UI.Timer.Bindings.Window(INotifier.Default, UserSettings.Default);
        var window = new UI.Timer.Window() { DataContext = context, };

        this.ShutdownMode = ShutdownMode.OnMainWindowClose;
        this.MainWindow = window;
        this.MainWindow.Show();
    }

    partial void Break();
}
