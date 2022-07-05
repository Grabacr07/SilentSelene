using System.Windows.Input;
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
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);

        if (e.ButtonState == MouseButtonState.Pressed) this.DragMove();
    }
}
