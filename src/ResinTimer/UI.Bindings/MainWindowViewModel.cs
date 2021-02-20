using MetroTrilithon.Mvvm;
using ResinTimer.Properties;

namespace ResinTimer.UI.Bindings
{
    public class MainWindowViewModel : WindowViewModel
    {
        public MainWindowViewModel()
        {
            this.Title = AssemblyInfo.Product;
        }
    }
}
