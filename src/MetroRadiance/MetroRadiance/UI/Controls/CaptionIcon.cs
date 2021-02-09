using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using MetroRadiance.Interop.Win32;

namespace MetroRadiance.UI.Controls
{
    public class CaptionIcon : Button
    {
        static CaptionIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CaptionIcon), new FrameworkPropertyMetadata(typeof(CaptionIcon)));
        }

        private bool _isSystemMenuOpened;


        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            var window = Window.GetWindow(this);
            if (window == null) return;

            window.SourceInitialized += this.Initialize;
        }

        private void Initialize(object? sender, EventArgs e)
        {
            if (!(sender is Window window)) return;
            
            window.SourceInitialized -= this.Initialize;

            if (PresentationSource.FromVisual(window) is HwndSource source)
            {
                source.AddHook(this.WndProc);
                window.Closed += (o, args) => source.RemoveHook(this.WndProc);
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == (int)WindowsMessages.WM_NCLBUTTONDOWN)
            {
                this._isSystemMenuOpened = false;
            }

            return IntPtr.Zero;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (!(Window.GetWindow(this) is MetroWindow window))
            {
                base.OnMouseDown(e);
                return;
            }

            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount == 1)
                {
                    if (!this._isSystemMenuOpened)
                    {
                        this._isSystemMenuOpened = true;
                        var point = this.PointToScreen(new Point(0, this.ActualHeight));
                        SystemCommands.ShowSystemMenu(window, new Point(point.X / window.CurrentDpi.ScaleX, point.Y / window.CurrentDpi.ScaleY));
                    }
                    else
                    {
                        this._isSystemMenuOpened = false;
                    }
                }
                else if (e.ClickCount == 2)
                {
                    window.Close();
                }
            }
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            if (!(Window.GetWindow(this) is MetroWindow window))
            {
                base.OnMouseRightButtonUp(e);
                return;
            }

            var point = this.PointToScreen(e.GetPosition(this));
            SystemCommands.ShowSystemMenu(window, new Point(point.X / window.CurrentDpi.ScaleX, point.Y / window.CurrentDpi.ScaleY));
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            this._isSystemMenuOpened = false;
            base.OnMouseLeave(e);
        }
    }
}
