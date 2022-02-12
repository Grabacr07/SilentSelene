using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SilentSelene.UI.Controls;

public class RemainingConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is TimeSpan span
            ? $"{span.Days * 24 + span.Hours:D2}:{span.Minutes:D2}:{span.Seconds:D2}"
            : "";

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
