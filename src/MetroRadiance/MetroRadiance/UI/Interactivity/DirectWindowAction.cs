using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MetroRadiance.Internal;
using MetroRadiance.UI.Controls;
using Microsoft.Xaml.Behaviors;

namespace MetroRadiance.UI.Interactivity
{
    internal class DirectWindowAction : TriggerAction<FrameworkElement>
    {
        #region WindowAction 依存関係プロパティ

        public static readonly DependencyProperty WindowActionProperty
            = DependencyProperty.Register(
                nameof(WindowAction),
                typeof(WindowAction),
                typeof(DirectWindowAction),
                new UIPropertyMetadata(WindowAction.Active));

        public WindowAction WindowAction
        {
            get => (WindowAction)this.GetValue(WindowActionProperty);
            set => this.SetValue(WindowActionProperty, value);
        }

        #endregion

        protected override void Invoke(object parameter)
        {
            this.WindowAction.Invoke(this.AssociatedObject);
        }
    }
}
