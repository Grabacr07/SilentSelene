using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MetroRadiance.Internal;

namespace MetroRadiance.UI.Controls
{
    public class ProgressRing : Control
    {
        static ProgressRing()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressRing), new FrameworkPropertyMetadata(typeof(ProgressRing)));
        }

        #region IsActive dependency property

        public static readonly DependencyProperty IsActiveProperty
            = DependencyProperty.Register(
                nameof(IsActive),
                typeof(bool),
                typeof(ProgressRing),
                new PropertyMetadata(BooleanBoxes.TrueBox, IsActiveChangedCallback));

        private static void IsActiveChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((ProgressRing)d).SetActivate((bool)args.NewValue);
        }

        public bool IsActive
        {
            get => (bool)this.GetValue(IsActiveProperty);
            set => this.SetValue(IsActiveProperty, BooleanBoxes.Box(value));
        }

        #endregion

        #region EllipseDiameter dependency property

        public static readonly DependencyProperty EllipseDiameterProperty
            = DependencyProperty.Register(
                nameof(EllipseDiameter),
                typeof(int),
                typeof(ProgressRing),
                new PropertyMetadata(3));

        public int EllipseDiameter
        {
            get => (int)this.GetValue(EllipseDiameterProperty);
            set => this.SetValue(EllipseDiameterProperty, value);
        }

        #endregion

        #region EllipseOffset dependency property

        public static readonly DependencyProperty EllipseOffsetProperty
            = DependencyProperty.Register(
                nameof(EllipseOffset),
                typeof(Thickness),
                typeof(ProgressRing),
                new PropertyMetadata(new Thickness(0, 7, 0, 0)));

        public Thickness EllipseOffset
        {
            get => (Thickness)this.GetValue(EllipseOffsetProperty);
            set => this.SetValue(EllipseOffsetProperty, value);
        }

        #endregion

        #region MaxSideLength dependency property

        public static readonly DependencyProperty MaxSideLengthProperty
            = DependencyProperty.Register(
                nameof(MaxSideLength),
                typeof(int),
                typeof(ProgressRing),
                new PropertyMetadata(20));

        public int MaxSideLength
        {
            get => (int)this.GetValue(MaxSideLengthProperty);
            set => this.SetValue(MaxSideLengthProperty, value);
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.SetActivate(true);
            this.SetSize(true);
        }

        private void SetActivate(bool active)
        {
            VisualStateManager.GoToState(this, active ? "Active" : "Inactive", true);
        }

        private void SetSize(bool large)
        {
            VisualStateManager.GoToState(this, large ? "Large" : "Small", true);
        }
    }
}
