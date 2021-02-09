using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WindowsDesktopApp.UI.Hosting
{
    public class MainContentCategoryHost
    {
        public string CategoryName { get; }

        public ObservableCollection<MainContent> Contents { get; }

        public MainContentCategoryHost(string categoryName, IEnumerable<MainContent> contents)
        {
            this.CategoryName = categoryName;
            this.Contents = new ObservableCollection<MainContent>(contents);
        }
    }
}
