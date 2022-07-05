using System.Windows;
using Wpf.Ui.Appearance;

namespace SilentSelene.UI.Preferences;

partial class Window
{
    public Window()
    {
        this.InitializeComponent();
        Wpf.Ui.Appearance.Background.Apply(this, BackgroundType.Mica);
    }

    private void HandleRootNavigationLoaded(object sender, RoutedEventArgs e)
    {
        this.RootNavigation.Navigate("generals");
    }
}
