using System.Windows;
using Livet.Messaging;
using MetroRadiance.UI.Controls;

namespace MetroTrilithon.UI.Interactivity
{
    public class FlyoutMessage : InteractionMessage
    {
        #region Content dependency property

        public static readonly DependencyProperty ContentProperty
            = DependencyProperty.Register(
                nameof(Content),
                typeof(ModalFlyoutMessage),
                typeof(FlyoutMessage),
                new PropertyMetadata(default(ModalFlyoutMessage)));

        public ModalFlyoutMessage Content
        {
            get => (ModalFlyoutMessage)this.GetValue(ContentProperty);
            set => this.SetValue(ContentProperty, value);
        }

        #endregion

        public FlyoutMessage()
        {
        }

        public FlyoutMessage(string messageKey, ModalFlyoutMessage content)
            : base(messageKey)
        {
            this.Content = content;
        }

        protected override Freezable CreateInstanceCore()
            => new FlyoutMessage(this.MessageKey, this.Content);
    }
}
