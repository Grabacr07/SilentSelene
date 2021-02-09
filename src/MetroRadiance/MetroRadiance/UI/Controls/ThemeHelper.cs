using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MetroRadiance.Internal;

namespace MetroRadiance.UI.Controls
{
    public static class ThemeHelper
    {
        private static readonly Dictionary<FrameworkElement, IDisposable> _registeredElements = new Dictionary<FrameworkElement, IDisposable>();

        #region HasThemeResources attached property

        public static readonly DependencyProperty HasThemeResourcesProperty
            = DependencyProperty.RegisterAttached(
                "HasThemeResources",
                typeof(bool),
                typeof(ThemeService),
                new PropertyMetadata(BooleanBoxes.FalseBox, HasThemeResourcesChangedCallback));

        private static void HasThemeResourcesChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (DesignerProperties.GetIsInDesignMode(d)) return;

            var element = (FrameworkElement)d;
            var oldValue = (bool)args.OldValue;
            var newValue = (bool)args.NewValue;

            void Logic()
            {
                if (oldValue && !newValue)
                {
                    // true -> false
                    if (_registeredElements.TryGetValue(element, out var disposable))
                    {
                        _registeredElements.Remove(element);
                        disposable.Dispose();
                    }
                }
                else if (!oldValue && newValue)
                {
                    // false -> true
                    _registeredElements[element] = ThemeService.Current.Register(element.Resources);
                }
            }

            if (element.IsInitialized)
            {
                Logic();
            }
            else
            {
                element.Initialized += Handler;

                void Handler(object? sender, EventArgs e)
                {
                    element.Initialized -= Handler;
                    Logic();
                }
            }
        }

        public static void SetHasThemeResources(FrameworkElement element, bool value)
            => element.SetValue(HasThemeResourcesProperty, value);

        public static bool GetHasThemeResources(FrameworkElement element)
            => (bool)element.GetValue(HasThemeResourcesProperty);

        #endregion
    }
}
