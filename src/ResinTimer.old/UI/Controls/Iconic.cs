using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ResinTimer.UI.Controls
{
    public static class Iconic
    {
        #region Content attached property

        public static readonly DependencyProperty ContentProperty
            = DependencyProperty.RegisterAttached(
                "Content",
                typeof(string),
                typeof(Iconic),
                new PropertyMetadata(default(string), HandleContentChanged));

        private static void HandleContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ContentControl button)) return;
            if (!(e.NewValue is string s)) return;

            var split = s.Split("|");
            if (split.Length < 1) return;

            var panel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
            };
            var icon = new TextBlock()
            {
                Text = split[0],
                FontFamily = new FontFamily("Segoe MDL2 Assets"),
                FontSize = button.FontSize,
                Margin = new Thickness(4),
            };
            panel.Children.Add(icon);

            if (split.Length >= 2)
            {
                var text = new TextBlock()
                {
                    Text = split[1],
                    FontSize = button.FontSize,
                    Margin = new Thickness(4, 3, 4, 4),
                };
                panel.Children.Add(text);
            }

            button.Content = panel;
        }

        public static void SetContent(ContentControl element, string value)
            => element.SetValue(ContentProperty, value);

        public static string GetContent(ContentControl element)
            => (string)element.GetValue(ContentProperty);

        #endregion
    }
}
