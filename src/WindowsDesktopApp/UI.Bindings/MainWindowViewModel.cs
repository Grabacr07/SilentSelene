using WindowsDesktopApp.Properties;
using MetroTrilithon.Mvvm;

namespace WindowsDesktopApp.UI.Bindings
{
    public class MainWindowViewModel : WindowViewModel
    {
        public MainWindowViewModel()
        {
            this.Title = AssemblyInfo.Title;
        }
    }
}
