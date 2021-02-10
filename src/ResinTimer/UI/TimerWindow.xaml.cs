using System;
using System.Windows.Input;

namespace ResinTimer.UI
{
    public partial class TimerWindow
    {
        public TimerWindow()
        {
            this.InitializeComponent();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }
    }
}
