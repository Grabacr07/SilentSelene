using System.Windows;
using WPFUI.Appearance;

namespace SilentSelene.UI.Preferences;

partial class Window
{
    public Window()
    {
        this.InitializeComponent();
        WPFUI.Appearance.Background.Apply(this, BackgroundType.Mica);
    }

    private void HandleRootNavigationLoaded(object sender, RoutedEventArgs e)
    {
        this.RootNavigation.Navigate("generals");
    }
}
