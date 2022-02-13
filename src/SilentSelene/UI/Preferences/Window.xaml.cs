using System.Windows;

namespace SilentSelene.UI.Preferences;

partial class Window
{
    public Window()
    {
        WPFUI.Background.Manager.Apply(this);
        this.InitializeComponent();
    }

    private void HandleRootNavigationLoaded(object sender, RoutedEventArgs e)
    {
        this.RootNavigation.Navigate("generals");
    }
}
