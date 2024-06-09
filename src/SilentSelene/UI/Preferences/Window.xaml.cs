using System.Windows;
using Wpf.Ui.Controls;

namespace SilentSelene.UI.Preferences;

partial class Window
{
    public Window()
    {
        this.InitializeComponent();
    }

    private void HandleRootNavigationLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is NavigationView view) view.Navigate("generals");
    }
}
