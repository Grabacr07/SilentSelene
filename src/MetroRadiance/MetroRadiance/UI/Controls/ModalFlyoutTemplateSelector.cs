using System;
using System.Windows;
using System.Windows.Controls;

namespace MetroRadiance.UI.Controls
{
    public class ModalFlyoutTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? ModalFlyoutMessageTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var template = item switch
            {
                ModalFlyoutMessage _ => this.ModalFlyoutMessageTemplate,
                _ => null,
            };

            return template ?? base.SelectTemplate(item, container);
        }
    }
}
