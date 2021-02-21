using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using MetroRadiance.Internal;
using MetroRadiance.Interop.Win32;

namespace MetroRadiance.UI.Controls
{
    /// <summary>
    /// ウィンドウのキャプション部分で使用するために最適化された <see cref="Button"/> コントロールを表します。
    /// </summary>
    public class CaptionButton : Button
    {
        static CaptionButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CaptionButton), new FrameworkPropertyMetadata(typeof(CaptionButton)));
        }

        private Window _owner = default!;

        #region WindowAction dependency property

        public static readonly DependencyProperty WindowActionProperty
            = DependencyProperty.Register(
                nameof(WindowAction),
                typeof(WindowAction),
                typeof(CaptionButton),
                new UIPropertyMetadata(WindowAction.None));

        /// <summary>
        /// ボタンに割り当てるウィンドウ操作を取得または設定します。
        /// </summary>
        public WindowAction WindowAction
        {
            get => (WindowAction)this.GetValue(WindowActionProperty);
            set => this.SetValue(WindowActionProperty, value);
        }

        #endregion

        #region Mode dependency property

        public static readonly DependencyProperty ModeProperty
            = DependencyProperty.Register(
                nameof(Mode),
                typeof(CaptionButtonMode),
                typeof(CaptionButton),
                new UIPropertyMetadata(CaptionButtonMode.Normal));

        public CaptionButtonMode Mode
        {
            get => (CaptionButtonMode)this.GetValue(ModeProperty);
            set => this.SetValue(ModeProperty, value);
        }

        #endregion

        #region IsChecked dependency property

        public static readonly DependencyProperty IsCheckedProperty
            = DependencyProperty.Register(
                nameof(IsChecked),
                typeof(bool),
                typeof(CaptionButton),
                new UIPropertyMetadata(BooleanBoxes.FalseBox));

        public bool IsChecked
        {
            get => (bool)this.GetValue(IsCheckedProperty);
            set => this.SetValue(IsCheckedProperty, BooleanBoxes.Box(value));
        }

        #endregion

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this._owner = Window.GetWindow(this);

            if (this._owner != null)
            {
                this._owner.StateChanged += (sender, args) => this.ChangeVisibility();
                this._owner.SourceInitialized += (sender, args) => this.ChangeVisibility();
            }
        }

        protected override void OnClick()
        {
            this.WindowAction.Invoke(this);

            if (this.Mode == CaptionButtonMode.Toggle) this.IsChecked = !this.IsChecked;

            base.OnClick();
        }

        private void ChangeVisibility()
        {
            switch (this.WindowAction)
            {
                case WindowAction.Maximize:
                    this.Visibility = this._owner.WindowState != WindowState.Maximized && this.HasWindowStyle(WindowStyles.WS_MAXIMIZEBOX)
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                    break;
                case WindowAction.Minimize:
                    this.Visibility = this._owner.WindowState != WindowState.Minimized && this.HasWindowStyle(WindowStyles.WS_MINIMIZEBOX)
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                    break;
                case WindowAction.Restore:
                    this.Visibility = this._owner.WindowState != WindowState.Normal
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                    break;
            }
        }

        private bool HasWindowStyle(WindowStyles style)
        {
            return (User32.GetWindowLong(new WindowInteropHelper(this._owner).Handle) & style) == style;
        }
    }
}
