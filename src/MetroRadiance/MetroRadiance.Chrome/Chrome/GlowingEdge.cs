using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using MetroRadiance.Chrome.Primitives;
using MetroRadiance.Internal;

namespace MetroRadiance.Chrome
{
    [TemplatePart(Name = PART_GradientBrush, Type = typeof(GradientBrush))]
    public class GlowingEdge : Control, IValueConverter
    {
        private const string PART_GradientBrush = nameof(PART_GradientBrush);

        #region Infrastructures

        // ReSharper disable InconsistentNaming

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static GridLength __Thickness => new GridLength(ChromeWindow.Thickness);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static double __CornerThickness => ChromeWindow.Thickness * 2;

        // ReSharper restore InconsistentNaming

        #endregion

        static GlowingEdge()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GlowingEdge), new FrameworkPropertyMetadata(typeof(GlowingEdge)));
        }

        #region Position dependency property

        public static readonly DependencyProperty PositionProperty
            = DependencyProperty.Register(
                nameof(Position),
                typeof(Dock),
                typeof(GlowingEdge),
                new PropertyMetadata(default(Dock)));

        public Dock Position
        {
            get => (Dock)this.GetValue(PositionProperty);
            set => this.SetValue(PositionProperty, value);
        }

        #endregion

        #region CanResize dependency property

        public static readonly DependencyProperty CanResizeProperty
            = DependencyProperty.Register(
                nameof(CanResize),
                typeof(bool),
                typeof(GlowingEdge),
                new PropertyMetadata(BooleanBoxes.TrueBox));

        public bool CanResize
        {
            get => (bool)this.GetValue(CanResizeProperty);
            set => this.SetValue(CanResizeProperty, BooleanBoxes.Box(value));
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var num = 1;
            if (this.GetTemplateChild(PART_GradientBrush) is GradientBrush brush)
            {
                this.SetGradientStops(brush);
                num++;
            }

            while (this.GetTemplateChild(PART_GradientBrush + num) is GradientBrush brush2)
            {
                this.SetGradientStops(brush2);
                num++;
            }
        }

        private void SetGradientStops(GradientBrush brush)
        {
            var stops = new GradientStopCollection();
            var options = new (double offset, double opacity)[]
            {
                (1.0, 0.005),
                (0.8, 0.020),
                (0.6, 0.040),
                (0.4, 0.080),
                (0.2, 0.160),
                (0.1, 0.260),
                (0.0, 0.360),
            };

            foreach (var (offset, opacity) in options)
            {
                var stop = new GradientStop
                {
                    Offset = offset,
                };
                var binding = new Binding(nameof(this.Background))
                {
                    Source = this,
                    Converter = this,
                    ConverterParameter = opacity,
                };
                BindingOperations.SetBinding(stop, GradientStop.ColorProperty, binding);
                stops.Add(stop);
            }

            brush.GradientStops = stops;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color;
            switch (value)
            {
                case Color c:
                    color = c;
                    break;

                case SolidColorBrush brush:
                    color = brush.Color;
                    break;

                default:
                    return Colors.Transparent;
            }

            if (double.TryParse(parameter?.ToString(), out var opacity) == false)
            {
                return color;
            }

            color.A = (byte)(color.A * opacity);
            return color;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
