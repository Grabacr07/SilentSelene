using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MetroTrilithon.UI.Interop;
using SilentSelene.Properties;
using Wpf.Ui.Appearance;

namespace SilentSelene.UI.Timer;

partial class Window
{
    public Window()
    {
        this.InitializeComponent();
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        Watcher.Watch(this);

        if (UserSettings.Default.WindowPlacement != new Point(0, 0))
        {
            var rect = new Rect(UserSettings.Default.WindowPlacement, this.RenderSize);
            var placement = new WindowPlacement(WindowState.Normal, rect);
            placement.Apply(this);
        }
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);

        if (e.ButtonState == MouseButtonState.Pressed) this.DragMove();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        UserSettings.Default.WindowPlacement = WindowPlacement.Get(this).Rect.TopLeft;
        UserSettings.Default.Save();
    }
}
