using System;
using System.Windows;
using System.Windows.Controls;

namespace SilentSelene.UI.Controls
{
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
    }
}
