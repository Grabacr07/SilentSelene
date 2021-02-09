using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MetroRadiance.UI.Controls
{
    public class CaptionBarDefinition : RowDefinition
    {
        private bool _setupRequired = true;

        public CaptionBarDefinition()
        {
            this.Loaded += (sender, args) =>
            {
                if (this._setupRequired && Window.GetWindow(this) is MetroWindow window)
                {
                    var binding = new Binding(nameof(MetroWindow.CaptionBarHeight))
                    {
                        Source = window,
                        Mode = BindingMode.OneWay,
                        Converter = new Converter(),
                    };
                    this.SetBinding(HeightProperty, binding);
                    this._setupRequired = false;
                }
            };

            this.SetupForDesignMode();
        }

        [Conditional("DEBUG")]
        private void SetupForDesignMode()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                this.Height = new GridLength(48.0, GridUnitType.Pixel);
            }
        }

        public class Converter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
                => value is double d
                    ? new GridLength(d, GridUnitType.Pixel)
                    : GridLength.Auto;

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                => throw new NotSupportedException();
        }
    }
}
