using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Livet.Behaviors.Messaging;
using Livet.Messaging;
using MetroRadiance.UI.Controls;

namespace MetroTrilithon.UI.Interactivity
{
    public class FlyoutMessageAction : InteractionMessageAction<DependencyObject>
    {
        protected override void InvokeAction(InteractionMessage m)
        {
            if (!(Window.GetWindow(this.AssociatedObject) is MetroWindow window)) return;
            if (!(m is FlyoutMessage message)) return;

            window.Flyout.Show(ModalFlyout.FromMessage(message.Content));
        }
    }
}
