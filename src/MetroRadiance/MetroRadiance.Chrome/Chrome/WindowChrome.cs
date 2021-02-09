using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MetroRadiance.Chrome.Primitives;
using MetroRadiance.Internal;
using MetroRadiance.Platform;

namespace MetroRadiance.Chrome
{
    /// <summary>
    /// ウィンドウにアタッチされ、四辺にカスタム UI を表示する機能を提供します。
    /// </summary>
    public class WindowChrome : DependencyObject
    {
        private static readonly HashSet<FrameworkElement> _sizingElements = new HashSet<FrameworkElement>();

        private readonly ChromePart _top = new ChromePart(new TopChromeWindow(), Dock.Top);
        private readonly ChromePart _left = new ChromePart(new LeftChromeWindow(), Dock.Left);
        private readonly ChromePart _right = new ChromePart(new RightChromeWindow(), Dock.Right);
        private readonly ChromePart _bottom = new ChromePart(new BottomChromeWindow(), Dock.Bottom);

        #region Content wrappers

        public object Left
        {
            get => this._left.Content;
            set => this._left.Content = value;
        }

        public object Right
        {
            get => this._right.Content;
            set => this._right.Content = value;
        }

        public object Top
        {
            get => this._top.Content;
            set => this._top.Content = value;
        }

        public object Bottom
        {
            get => this._bottom.Content;
            set => this._bottom.Content = value;
        }

        #endregion

        #region BorderThickness dependency property

        public static readonly DependencyProperty BorderThicknessProperty
            = DependencyProperty.Register(
                nameof(BorderThickness),
                typeof(Thickness),
                typeof(WindowChrome),
                new PropertyMetadata(new Thickness(.99), BorderThicknessPropertyCallback));

        private static void BorderThicknessPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (WindowChrome)d;
            var newValue = (Thickness)e.NewValue;

            instance.UpdateThickness(newValue);
        }

        public Thickness BorderThickness
        {
            get => (Thickness)this.GetValue(BorderThicknessProperty);
            set => this.SetValue(BorderThicknessProperty, value);
        }

        #endregion

        #region CanResize dependency property

        public static readonly DependencyProperty CanResizeProperty
            = DependencyProperty.Register(
                nameof(CanResize),
                typeof(bool),
                typeof(WindowChrome),
                new PropertyMetadata(GlowingEdge.CanResizeProperty.DefaultMetadata.DefaultValue, CanResizePropertyCallback));

        private static void CanResizePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (WindowChrome)d;
            var newValue = (bool)e.NewValue;

            instance._top.Edge.CanResize = newValue;
            instance._left.Edge.CanResize = newValue;
            instance._right.Edge.CanResize = newValue;
            instance._bottom.Edge.CanResize = newValue;
        }

        public bool CanResize
        {
            get => (bool)this.GetValue(CanResizeProperty);
            set => this.SetValue(CanResizeProperty, BooleanBoxes.Box(value));
        }

        #endregion

        #region OverrideDefaultEdge dependency property

        public static readonly DependencyProperty OverrideDefaultEdgeProperty
            = DependencyProperty.Register(
                nameof(OverrideDefaultEdge),
                typeof(bool),
                typeof(WindowChrome),
                new PropertyMetadata(BooleanBoxes.FalseBox, OverrideDefaultEdgePropertyCallback));

        private static void OverrideDefaultEdgePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (WindowChrome)d;
            var oldValue = (bool)e.OldValue;
            var newValue = (bool)e.NewValue;

            if (!oldValue && newValue)
            {
                // false -> true
                instance._top.Edge.Visibility = Visibility.Collapsed;
                instance._left.Edge.Visibility = Visibility.Collapsed;
                instance._right.Edge.Visibility = Visibility.Collapsed;
                instance._bottom.Edge.Visibility = Visibility.Collapsed;
            }

            if (oldValue && !newValue)
            {
                // true -> false
                instance._top.Edge.Visibility = Visibility.Visible;
                instance._left.Edge.Visibility = Visibility.Visible;
                instance._right.Edge.Visibility = Visibility.Visible;
                instance._bottom.Edge.Visibility = Visibility.Visible;
            }
        }

        public bool OverrideDefaultEdge
        {
            get => (bool)this.GetValue(OverrideDefaultEdgeProperty);
            set => this.SetValue(OverrideDefaultEdgeProperty, BooleanBoxes.Box(value));
        }

        #endregion

        #region SizingMode attached property

        public static readonly DependencyProperty SizingModeProperty
            = DependencyProperty.RegisterAttached(
                "SizingMode",
                typeof(SizingMode),
                typeof(WindowChrome),
                new PropertyMetadata(SizingMode.None, SizingModeChangedCallback));

        private static void SizingModeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FrameworkElement element)) return;

            var newValue = (SizingMode)e.NewValue;
            if (newValue != SizingMode.None)
            {
                if (_sizingElements.Add(element))
                {
                    element.PreviewMouseLeftButtonDown += SizingElementButtonDownCallback;
                }
            }
            else
            {
                if (_sizingElements.Remove(element))
                {
                    element.PreviewMouseLeftButtonDown -= SizingElementButtonDownCallback;
                }
            }
        }

        private static void SizingElementButtonDownCallback(object sender, MouseButtonEventArgs args)
        {
            if (!(sender is FrameworkElement element)) return;

            (Window.GetWindow(element) as ChromeWindow)?.Resize(GetSizingMode(element));
        }

        public static void SetSizingMode(FrameworkElement element, SizingMode value)
            => element.SetValue(SizingModeProperty, value);

        public static SizingMode GetSizingMode(FrameworkElement element)
            => (SizingMode)element.GetValue(SizingModeProperty);

        #endregion

        #region Instance attached property

        public static readonly DependencyProperty InstanceProperty
            = DependencyProperty.RegisterAttached(
                "Instance",
                typeof(WindowChrome),
                typeof(WindowChrome),
                new PropertyMetadata(default(WindowChrome), InstanceChangedCallback));

        private static void InstanceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Window window)) return;

            var oldValue = (WindowChrome)e.OldValue;
            var newValue = (WindowChrome)e.NewValue;

            oldValue?.Detach();
            newValue?.Attach(window);
        }

        public static void SetInstance(Window window, WindowChrome value)
            => window.SetValue(InstanceProperty, value);

        public static WindowChrome GetInstance(Window window)
            => (WindowChrome)window.GetValue(InstanceProperty);

        #endregion

        public WindowChrome()
        {
            this.UpdateThickness(this.BorderThickness);
        }

        /// <summary>
        /// 指定した WPF <see cref="Window"/> に、このクローム UI をアタッチします。
        /// </summary>
        public void Attach(Window window)
        {
            this.Detach();

            this._top.Window.Attach(window);
            this._left.Window.Attach(window);
            this._right.Window.Attach(window);
            this._bottom.Window.Attach(window);

            this.CanResize = true;
        }

        /// <summary>
        /// 指定したウィンドウに、このクローム UI をアタッチします。
        /// </summary>
        public void Attach(IChromeOwner window)
        {
            this.Detach();

            this._top.Window.Attach(window);
            this._left.Window.Attach(window);
            this._right.Window.Attach(window);
            this._bottom.Window.Attach(window);

            this.CanResize = false;
        }

        public void Detach()
        {
            this._top.Window.Detach();
            this._left.Window.Detach();
            this._right.Window.Detach();
            this._bottom.Window.Detach();
        }

        public void Close()
        {
            this.Detach();

            this._top.Window.Close();
            this._left.Window.Close();
            this._right.Window.Close();
            this._bottom.Window.Close();
        }

        private void UpdateThickness(Thickness thickness)
        {
            this._top.Edge.BorderThickness = thickness;
            this._left.Edge.BorderThickness = thickness;
            this._right.Edge.BorderThickness = thickness;
            this._bottom.Edge.BorderThickness = thickness;

            var offset = new Thickness(
                ChromeWindow.Thickness + thickness.Left,
                ChromeWindow.Thickness + thickness.Top,
                ChromeWindow.Thickness + thickness.Right,
                ChromeWindow.Thickness + thickness.Bottom);

            this._top.Window.Offset = offset;
            this._left.Window.Offset = offset;
            this._right.Window.Offset = offset;
            this._bottom.Window.Offset = offset;
        }

        private class ChromePart
        {
            private readonly ContentControl _customContentHost;

            public GlowingEdge Edge { get; }

            public ChromeWindow Window { get; }

            public object Content
            {
                get => this._customContentHost.Content;
                set
                {
                    if (this._customContentHost.Content == value) return;

                    this._customContentHost.Content = value;
                    this.Window?.Update();
                }
            }

            public ChromePart(ChromeWindow window, Dock position)
            {
                this._customContentHost = new ContentControl();
                this.Edge = new GlowingEdge
                {
                    Position = position,
                };

                var grid = new Grid();
                grid.Children.Add(this.Edge);
                grid.Children.Add(this._customContentHost);

                this.Window = window;
                this.Window.Content = grid;
            }
        }
    }
}
