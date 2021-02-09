using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using MetroTrilithon.Mvvm;

namespace ResinTimer.UI.Hosting
{
    public class MainContentHost : Notifier
    {
        public static MainContentHost Instance { get; }
            = new MainContentHost();
        
        public ObservableCollection<MainContentCategoryHost> Categories { get; }

        #region Current notification property

        private MainContent? _Current;

        public MainContent? Current
        {
            get => this._Current;
            set
            {
                if (this._Current == value) return;

                // SelectedItem に null 突っ込むと選択解除される ListBox の仕様を利用して、
                // 複数の ListBox で SelectedItem のバインディング ソースを共用する
                this._Current = null;
                this.RaisePropertyChanged();

                if (value != null)
                {
                    this._Current = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        private MainContentHost()
        {
            var categories = this.CreateContents()
                .GroupBy(x => Deconstruct(x.Category))
                .OrderBy(x => x.Key.order)
                .Select(x => new MainContentCategoryHost(x.Key.name, x.OrderBy(y => y.Order)));

            this.Categories = new ObservableCollection<MainContentCategoryHost>(categories);
            this.Current = this.Categories.First().Contents.First();

            static (int order, string name) Deconstruct(string name)
            {
                var values = name.Split(":", StringSplitOptions.RemoveEmptyEntries);
                return values.Length < 2 || int.TryParse(values[0], out var order) == false
                    ? (int.MaxValue, name)
                    : (order, values[1]);
            }
        }

        public IEnumerable<MainContent<T>> EnumerateContents<T>()
            => this.Categories
                .SelectMany(x => x.Contents)
                .Where(x => x.Instance is FrameworkElement element && element.DataContext is T)
                .Select(x => new MainContent<T>(x, (T)((FrameworkElement)x.Instance).DataContext));

        private IEnumerable<MainContent> CreateContents()
        {
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()))
            {
                var attribute = type.GetCustomAttribute<MainContentAttribute>();
                if (attribute == null) continue;

                yield return new MainContent(type, attribute, this.Activate);
            }
        }

        private void Activate(MainContent target)
        {
            if (this.Categories.SelectMany(x => x.Contents).Any(x => x == target))
            {
                this.Current = target;
            }
        }
    }
}
