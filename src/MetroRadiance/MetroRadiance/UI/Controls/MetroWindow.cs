using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using MetroRadiance.Internal;
using MetroRadiance.Interop;
using MetroRadiance.Interop.Win32;
using MetroRadiance.Platform;
using ShellChrome = System.Windows.Shell.WindowChrome;
using MetroChrome = MetroRadiance.Chrome.WindowChrome;

namespace MetroRadiance.UI.Controls
{
    /// <summary>
    /// Metro スタイル風のウィンドウを表します。
    /// </summary>
    [TemplatePart(Name = PART_ContentHost, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PART_FlyoutHost, Type = typeof(FlyoutHost))]
    [TemplatePart(Name = PART_CaptionBar, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PART_ResizeGrip, Type = typeof(FrameworkElement))]
    public class MetroWindow : Window
    {
        private const string PART_ContentHost = nameof(PART_ContentHost);
        private const string PART_FlyoutHost = nameof(PART_FlyoutHost);
        private const string PART_CaptionBar = nameof(PART_CaptionBar);
        private const string PART_ResizeGrip = nameof(PART_ResizeGrip);

        static MetroWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroWindow), new FrameworkPropertyMetadata(typeof(MetroWindow)));
        }

        /// <remarks>WPF が認識しているシステムの DPI (プライマリ モニターの DPI)。</remarks>
        private Dpi _systemDpi;

        /// <remarks>このウィンドウが表示されているモニターの現在の DPI。</remarks>
        internal Dpi CurrentDpi { get; set; }

        private IntPtr _handle;
        private HwndSource _source = default!;
        private FrameworkElement? _resizeGrip;
        private FrameworkElement? _captionBar;
        private ContentPresenter? _contentHost;
        private FlyoutHost? _flyoutHost;

        public FlyoutManager Flyout { get; }

        #region MetroChrome dependency property

        public static readonly DependencyProperty MetroChromeProperty
            = DependencyProperty.Register(
                nameof(MetroChrome),
                typeof(MetroChrome),
                typeof(MetroWindow),
                new PropertyMetadata(default(MetroChrome), HandleMetroChromeChanged));

        private static void HandleMetroChromeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var chrome = (MetroChrome)args.NewValue;
            var window = (Window)d;

            MetroChrome.SetInstance(window, chrome);
        }

        public MetroChrome MetroChrome
        {
            get => (MetroChrome)this.GetValue(MetroChromeProperty);
            set => this.SetValue(MetroChromeProperty, value);
        }

        #endregion

        #region DpiScaleTransform readonly dependency property

        internal static readonly DependencyPropertyKey InternalDpiScaleTransformProperty
            = DependencyProperty.RegisterReadOnly(
                nameof(DpiScaleTransform),
                typeof(Transform),
                typeof(MetroWindow),
                new PropertyMetadata(default(Transform)));

        public static readonly DependencyProperty DpiScaleTransformProperty
            = InternalDpiScaleTransformProperty.DependencyProperty;

        public Transform DpiScaleTransform
        {
            get => (Transform)this.GetValue(DpiScaleTransformProperty);
            internal set => this.SetValue(InternalDpiScaleTransformProperty, value);
        }

        #endregion

        #region IsRestoringWindowPlacement dependency property

        public static readonly DependencyProperty IsRestoringWindowPlacementProperty
            = DependencyProperty.Register(
                nameof(IsRestoringWindowPlacement),
                typeof(bool),
                typeof(MetroWindow),
                new UIPropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// ウィンドウの位置とサイズを復元できるようにするかどうかを示す値を取得または設定します。
        /// </summary>
        public bool IsRestoringWindowPlacement
        {
            get => (bool)this.GetValue(IsRestoringWindowPlacementProperty);
            set => this.SetValue(IsRestoringWindowPlacementProperty, BooleanBoxes.Box(value));
        }

        #endregion

        #region WindowSettings dependency property

        public static readonly DependencyProperty WindowSettingsProperty
            = DependencyProperty.Register(
                nameof(WindowSettings),
                typeof(IWindowSettings),
                typeof(MetroWindow),
                new UIPropertyMetadata(default(IWindowSettings)));

        /// <summary>
        /// ウィンドウの位置とサイズを保存または復元する方法を指定するオブジェクトを取得または設定します。
        /// </summary>
        public IWindowSettings? WindowSettings
        {
            get => (IWindowSettings)this.GetValue(WindowSettingsProperty);
            set => this.SetValue(WindowSettingsProperty, value);
        }

        #endregion

        #region HasDefaultCaptionBar dependency property

        public static readonly DependencyProperty HasDefaultCaptionBarProperty
            = DependencyProperty.Register(
                nameof(HasDefaultCaptionBar),
                typeof(bool),
                typeof(MetroWindow),
                new PropertyMetadata(BooleanBoxes.TrueBox));

        public bool HasDefaultCaptionBar
        {
            get => (bool)this.GetValue(HasDefaultCaptionBarProperty);
            set => this.SetValue(HasDefaultCaptionBarProperty, BooleanBoxes.Box(value));
        }

        #endregion

        #region CaptionBarHeight readonly dependency property

        internal static readonly DependencyPropertyKey InternalCaptionBarHeightProperty
            = DependencyProperty.RegisterReadOnly(
                nameof(CaptionBarHeight),
                typeof(double),
                typeof(MetroWindow),
                new PropertyMetadata(.0, HandleCaptionBarHeightChanged));

        public static readonly DependencyProperty CaptionBarHeightProperty
            = InternalCaptionBarHeightProperty.DependencyProperty;

        private static void HandleCaptionBarHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is MetroWindow window)) return;

            var chrome = ShellChrome.GetWindowChrome(window);
            if (chrome != null) chrome.CaptionHeight = (double)e.NewValue;
        }

        public double CaptionBarHeight
        {
            get => (double)this.GetValue(CaptionBarHeightProperty);
            internal set => this.SetValue(InternalCaptionBarHeightProperty, value);
        }

        #endregion

        #region IsAcrylic dependency property

        public static readonly DependencyProperty IsAcrylicProperty
            = DependencyProperty.Register(
                nameof(IsAcrylic),
                typeof(bool),
                typeof(MetroWindow),
                new PropertyMetadata(BooleanBoxes.FalseBox));

        public bool IsAcrylic
        {
            get => (bool)this.GetValue(IsAcrylicProperty);
            set => this.SetValue(IsAcrylicProperty, BooleanBoxes.Box(value));
        }

        #endregion

        #region AcrylicOpacity dependency property

        public static readonly DependencyProperty AcrylicOpacityProperty
            = DependencyProperty.Register(
                nameof(AcrylicOpacity),
                typeof(double),
                typeof(MetroWindow),
                new PropertyMetadata(0.85));

        public double AcrylicOpacity
        {
            get => (double)this.GetValue(AcrylicOpacityProperty);
            set => this.SetValue(AcrylicOpacityProperty, value);
        }

        #endregion

        #region AcrylicBackgroundOpacity readonly dependency property

        internal static readonly DependencyPropertyKey InternalAcrylicBackgroundOpacityProperty
            = DependencyProperty.RegisterReadOnly(
                nameof(AcrylicBackgroundOpacity),
                typeof(double),
                typeof(MetroWindow),
                new PropertyMetadata(1.0));

        public static readonly DependencyProperty AcrylicBackgroundOpacityProperty
            = InternalAcrylicBackgroundOpacityProperty.DependencyProperty;

        public double AcrylicBackgroundOpacity
        {
            get => (double)this.GetValue(AcrylicBackgroundOpacityProperty);
            internal set => this.SetValue(InternalAcrylicBackgroundOpacityProperty, value);
        }

        #endregion

        #region IsCaptionBar attached property

        public static readonly DependencyProperty IsCaptionBarProperty
            = DependencyProperty.RegisterAttached(
                "IsCaptionBar",
                typeof(bool),
                typeof(MetroWindow),
                new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.AffectsRender, IsCaptionBarChangedCallback));

        private static void IsCaptionBarChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FrameworkElement instance)) return;
            if (!(GetWindow(instance) is MetroWindow window)) return;

            if ((bool)e.NewValue)
            {
                window._captionBar = instance;
            }
        }

        public static void SetIsCaptionBar(FrameworkElement element, bool value)
            => element.SetValue(IsCaptionBarProperty, BooleanBoxes.Box(value));

        public static bool GetIsCaptionBar(FrameworkElement element)
            => (bool)element.GetValue(IsCaptionBarProperty);

        #endregion

        public MetroWindow()
        {
            this.MetroChrome = new MetroChrome();
            this.Flyout = new FlyoutManager();
            this.Loaded += (_, _) => this.WindowLoaded();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            if (PresentationSource.FromVisual(this) is HwndSource source)
            {
                this._handle = source.Handle;
                this._source = source;
                this._source.AddHook(this.WndProc);

                if (this.IsAcrylic)
                {
                    WindowComposition.Set(this, AccentState.ACCENT_ENABLE_BLURBEHIND, 0);
                    this.SetAcrylicBackground();
                }

                this._systemDpi = this.GetSystemDpi() ?? Dpi.Default;
                if (PerMonitorDpi.IsSupported)
                {
                    this.CurrentDpi = source.GetDpi();
                    this.ChangeDpi(this.CurrentDpi);
                }
                else
                {
                    this.CurrentDpi = this._systemDpi;
                }

                if (this.IsRestoringWindowPlacement)
                {
                    this.WindowSettings ??= new WindowSettings(this);
                    this.WindowSettings.Reload();

                    if (this.WindowSettings.Placement != null)
                    {
                        var placement = this.WindowSettings.Placement.Value;
                        placement.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
                        placement.flags = 0;
                        placement.showCmd = placement.showCmd == ShowWindowFlags.SW_SHOWMINIMIZED ? ShowWindowFlags.SW_SHOWNORMAL : placement.showCmd;

                        User32.SetWindowPlacement(source.Handle, ref placement);
                    }
                }
            }

            base.OnSourceInitialized(e);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.GetTemplateChild(PART_ContentHost) is ContentPresenter contentHost)
            {
                this._contentHost = contentHost;
                this._contentHost.SetBinding(
                    IsEnabledProperty,
                    new Binding(nameof(this.Flyout.ThroughInput)) { Source = this.Flyout });
            }

            if (this.GetTemplateChild(PART_FlyoutHost) is FlyoutHost flyoutHost)
            {
                this._flyoutHost = flyoutHost;
                this._flyoutHost.ItemsSource = this.Flyout.Items;
            }

            if (this.GetTemplateChild(PART_CaptionBar) is FrameworkElement captionBar)
            {
                if (this.HasDefaultCaptionBar)
                {
                    this._captionBar = captionBar;
                }
                else
                {
                    captionBar.Visibility = Visibility.Collapsed;
                }
            }

            if (this.GetTemplateChild(PART_ResizeGrip) is FrameworkElement resizeGrip)
            {
                this._resizeGrip = resizeGrip;
                this._resizeGrip.Visibility = this.ResizeMode == ResizeMode.CanResizeWithGrip
                    ? Visibility.Visible
                    : Visibility.Collapsed;

                ShellChrome.SetIsHitTestVisibleInChrome(resizeGrip, true);
            }
        }

        private void WindowLoaded()
        {
            if (this._captionBar != null)
            {
                this.CaptionBarHeight = this._captionBar.ActualHeight;
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (this._captionBar != null) this._captionBar.Opacity = 1.0;
            this.SetAcrylicBackground();
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);

            if (this._captionBar != null) this._captionBar.Opacity = 0.5;
            this.SetAcrylicBackground();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (!e.Cancel && this.WindowSettings != null)
            {
                User32.GetWindowPlacement(this._handle, out var placement);

                this.WindowSettings.Placement = this.IsRestoringWindowPlacement ? (WINDOWPLACEMENT?)placement : null;
                this.WindowSettings.Save();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            this._source?.RemoveHook(this.WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case (int)WindowsMessages.WM_NCHITTEST:
                {
                    if (this.ResizeMode == ResizeMode.CanResizeWithGrip
                        && this.WindowState == WindowState.Normal
                        && this._resizeGrip != null)
                    {
                        var ptScreen = lParam.ToPoint();
                        var ptClient = this._resizeGrip.PointFromScreen(ptScreen);

                        var rectTarget = new Rect(0, 0, this._resizeGrip.ActualWidth, this._resizeGrip.ActualHeight);
                        if (rectTarget.Contains(ptClient))
                        {
                            handled = true;
                            return (IntPtr)HitTestValues.HTBOTTOMRIGHT;
                        }
                    }

                    break;
                }

                case (int)WindowsMessages.WM_DPICHANGED:
                {
                    var dpiX = wParam.ToLoWord();
                    var dpiY = wParam.ToHiWord();
                    this.ChangeDpi(new Dpi(dpiX, dpiY));
                    handled = true;
                    break;
                }
            }

            return IntPtr.Zero;
        }

        private void ChangeDpi(Dpi dpi)
        {
            if (!PerMonitorDpi.IsSupported) return;

            this.DpiScaleTransform = dpi == this._systemDpi
                ? Transform.Identity
                : new ScaleTransform((double)dpi.X / this._systemDpi.X, (double)dpi.Y / this._systemDpi.Y);

            this.Width = this.Width * dpi.X / this.CurrentDpi.X;
            this.Height = this.Height * dpi.Y / this.CurrentDpi.Y;

            this.CurrentDpi = dpi;
        }

        private void SetAcrylicBackground()
        {
            this.AcrylicBackgroundOpacity = this.IsAcrylic && this.IsActive
                ? this.AcrylicOpacity
                : 1.0;
        }
    }
}
