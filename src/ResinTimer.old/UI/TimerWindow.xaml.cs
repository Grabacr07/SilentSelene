using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Core;
using ResinTimer.Properties;

namespace ResinTimer.UI
{
    partial class TimerWindow
    {
        public TimerWindow()
        {
            this.InitializeComponent();

            this.TaskbarIcon.LeftClickCommand = new ActionCommand(this.Restore);
            this.TaskbarIcon.SetBinding(VisibilityProperty, new Binding(nameof(this.Visibility))
            {
                Source = this,
                Converter = new ReverseVisibilityConverter(),
                Mode = BindingMode.OneWay,
            });
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        private void Restore()
        {
            switch (this.WindowState)
            {
                case WindowState.Minimized:
                    this.WindowState = WindowState.Normal;
                    break;

                default:
                    this.Show();
                    break;
            }
        }

        private void Hide(object sender, RoutedEventArgs e)
        {
            if (UserSettings.Default.ShowInTaskbar)
            {
                this.WindowState = WindowState.Minimized;
            }
            else
            {
                this.Hide();
            }
        }

        private class ReverseVisibilityConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
                => value is Visibility v
                    ? v switch
                    {
                        Visibility.Visible => Visibility.Collapsed,
                        _ => Visibility.Visible,
                    }
                    : Visibility.Visible;

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                => throw new NotSupportedException();
        }
    }
}
