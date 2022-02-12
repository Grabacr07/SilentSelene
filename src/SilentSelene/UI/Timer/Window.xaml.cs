using System.Windows.Input;

namespace SilentSelene.UI.Timer;

partial class Window
{
    public Window()
    {
        WPFUI.Background.Manager.Apply(this);
        this.InitializeComponent();
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);

        if (e.ButtonState == MouseButtonState.Pressed) this.DragMove();
    }
}
