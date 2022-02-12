using System;
using System.Windows;
using System.Windows.Controls;
using MetroTrilithon.UI;

namespace SilentSelene.UI.Controls
{
    public enum ResinSpinnerAppearance
    {
        Resin,
        Coin,
    }

    public class ResinSpinner : Control
    {
        static ResinSpinner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ResinSpinner), new FrameworkPropertyMetadata(typeof(ResinSpinner)));
        }

        #region Current dependency property

        public static readonly DependencyProperty CurrentProperty
            = DependencyProperty.Register(
                nameof(Current),
                typeof(int),
                typeof(ResinSpinner),
                new PropertyMetadata(default(int)));

        public int Current
        {
            get => (int)this.GetValue(CurrentProperty);
            set => this.SetValue(CurrentProperty, value);
        }

        #endregion

        #region Maximum dependency property

        public static readonly DependencyProperty MaximumProperty
            = DependencyProperty.Register(
                nameof(Maximum),
                typeof(int),
                typeof(ResinSpinner),
                new PropertyMetadata(default(int)));

        public int Maximum
        {
            get => (int)this.GetValue(MaximumProperty);
            set => this.SetValue(MaximumProperty, value);
        }

        #endregion

        #region Minimum dependency property

        public static readonly DependencyProperty MinimumProperty
            = DependencyProperty.Register(
                nameof(Minimum),
                typeof(int),
                typeof(ResinSpinner),
                new PropertyMetadata(default(int)));

        public int Minimum
        {
            get => (int)this.GetValue(MinimumProperty);
            set => this.SetValue(MinimumProperty, value);
        }

        #endregion

        #region RemainingTime dependency property

        public static readonly DependencyProperty RemainingTimeProperty
            = DependencyProperty.Register(
                nameof(RemainingTime),
                typeof(TimeSpan),
                typeof(ResinSpinner),
                new PropertyMetadata(default(TimeSpan)));

        public TimeSpan RemainingTime
        {
            get => (TimeSpan)this.GetValue(RemainingTimeProperty);
            set => this.SetValue(RemainingTimeProperty, value);
        }

        #endregion

        #region UseHighlight dependency property

        public static readonly DependencyProperty UseHighlightProperty
            = DependencyProperty.Register(
                nameof(UseHighlight),
                typeof(bool),
                typeof(ResinSpinner),
                new PropertyMetadata(BooleanBoxes.FalseBox));

        public bool UseHighlight
        {
            get => (bool)this.GetValue(UseHighlightProperty);
            set => this.SetValue(UseHighlightProperty, value);
        }

        #endregion

        #region Appearance dependency property

        public static readonly DependencyProperty AppearanceProperty
            = DependencyProperty.Register(
                nameof(Appearance),
                typeof(ResinSpinnerAppearance),
                typeof(ResinSpinner),
                new PropertyMetadata(default(ResinSpinnerAppearance)));

        public ResinSpinnerAppearance Appearance
        {
            get => (ResinSpinnerAppearance)this.GetValue(AppearanceProperty);
            set => this.SetValue(AppearanceProperty, value);
        }

        #endregion
    }
}
