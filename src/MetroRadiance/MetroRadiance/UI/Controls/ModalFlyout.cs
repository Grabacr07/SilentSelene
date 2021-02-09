using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MetroRadiance.UI.Controls
{
    public class ModalFlyout : ContentControl, IFlyout
    {
        static ModalFlyout()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ModalFlyout), new FrameworkPropertyMetadata(typeof(ModalFlyout)));
        }

        public bool IsOverlay
            => true;

        public event EventHandler? Close;

        protected void RaiseCloseEvent()
            => this.Close?.Invoke(this, EventArgs.Empty);

        public static ModalFlyout Create(
            string header,
            string body,
            Action? okAction,
            Action? cancelAction = null)
        {
            var message = new ModalFlyoutMessage(header, body, ModalFlyoutMessageCommand.CreateDefault(okAction), ModalFlyoutMessageCommand.CreateCancel(cancelAction));
            var flyout = new ModalFlyout { Content = message };
            foreach (var command in message.Commands) command.Executed += flyout.RaiseCloseEvent;

            return flyout;
        }

        public static ModalFlyout FromMessage(ModalFlyoutMessage message)
        {
            var flyout = new ModalFlyout() { Content = message };
            foreach (var command in message.Commands) command.Executed += flyout.RaiseCloseEvent;

            return flyout;
        }
    }
}
