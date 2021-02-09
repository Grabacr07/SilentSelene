using System;
using System.Windows;
using MetroTrilithon.Threading.Tasks;
using MetroTrilithon.UI;
using ResinTimer.Properties;
using ResinTimer.Tracking;

namespace ResinTimer
{
    public partial class Application
    {
        #region static

        public new static Application Current
            => (Application)System.Windows.Application.Current;

        static Application()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                if (args.ExceptionObject is Exception exception) Current.Tracker.TrackException(sender ?? AppDomain.CurrentDomain, exception);
            };
        }

        #endregion

        public ITracker Tracker { get; }

        public Application()
        {
#if DEBUG
            this.Tracker = new DebugTracker();
#else
            this.Tracker = new DefaultTracker();
#endif
            SettingsUtil.Update();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            TaskLog.Occured += (sender, args) => this.Tracker.TrackException(sender ?? typeof(TaskLog), args.Exception);
            UIDispatcher.Instance = this.Dispatcher;

            this.DispatcherUnhandledException += (sender, args) =>
            {
                this.Tracker.TrackException(sender, args.Exception);
                args.Handled = true;
            };

            this.Break();
        }

        partial void Break();
    }
}
