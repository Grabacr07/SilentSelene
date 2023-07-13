using System.Windows;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace SilentSelene.UI.Preferences;

partial class Window
{
    public Window()
    {
        this.InitializeComponent();
        Watcher.Watch(this);
    }

    private void HandleRootNavigationLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is NavigationView view) view.Navigate("generals");
    }
}
