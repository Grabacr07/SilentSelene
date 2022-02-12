using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ResinTimer.UI.Hosting
{
    public class MainContentHost : ContentControl
    {
        static MainContentHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(MainContentHost),
                new FrameworkPropertyMetadata(typeof(MainContentHost)));
        }

        private readonly SerialDisposable _windowCloseListener = new SerialDisposable();

        #region Categories dependency property

        public static readonly DependencyProperty CategoriesProperty
            = DependencyProperty.Register(
                nameof(Categories),
                typeof(IReadOnlyList<MainContentCategory>),
                typeof(MainContentHost),
                new PropertyMetadata(default(IReadOnlyList<MainContentCategory>)));

        public IReadOnlyList<MainContentCategory> Categories
        {
            get => (IReadOnlyList<MainContentCategory>)this.GetValue(CategoriesProperty);
            set => this.SetValue(CategoriesProperty, value);
        }

        #endregion

        #region Current dependency property

        public static readonly DependencyProperty CurrentProperty
            = DependencyProperty.Register(
                nameof(Current),
                typeof(MainContent),
                typeof(MainContentHost),
                new PropertyMetadata(default(MainContent), HandleCurrentPropertyChanged));

        public MainContent Current
        {
            get => (MainContent)this.GetValue(CurrentProperty);
            set => this.SetValue(CurrentProperty, value);
        }

        private static void HandleCurrentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is MainContentHost host)) return;

            foreach (var category in host.Categories) category.SelectedContent = (MainContent)e.NewValue;
        }

        #endregion

        #region HeaderWidth dependency property

        public static readonly DependencyProperty HeaderWidthProperty
            = DependencyProperty.Register(
                nameof(HeaderWidth),
                typeof(GridLength),
                typeof(MainContentHost),
                new PropertyMetadata(default(GridLength)));

        public GridLength HeaderWidth
        {
            get => (GridLength)this.GetValue(HeaderWidthProperty);
            set => this.SetValue(HeaderWidthProperty, value);
        }

        #endregion

        public MainContentHost()
        {
            var categories = this.CreateContents()
                .GroupBy(x => Deconstruct(x.Category))
                .OrderBy(x => x.Key.order)
                .Select(x => new MainContentCategory(this, x.Key.name, x.OrderBy(y => y.Order)));

            this.Categories = new List<MainContentCategory>(categories);
            this.Current = this.Categories.First().Contents.First();
            
            this.SetBinding(ContentProperty, new Binding()
            {
                Source = this,
                Path = new PropertyPath($"{nameof(this.Current)}.{nameof(this.Current.Instance)}"),
            });

            this.Loaded += (sender, args) =>
            {
                var window = Window.GetWindow(this);
                if (window == null) return;

                this._windowCloseListener.Disposable = Disposable.Create(() => window.Closed -= HandleWindowClosed);
                window.Closed += HandleWindowClosed;
            };

            static (int order, string name) Deconstruct(string name)
            {
                var values = name.Split(":", StringSplitOptions.RemoveEmptyEntries);
                return values.Length < 2 || int.TryParse(values[0], out var order) == false
                    ? (int.MaxValue, name)
                    : (order, values[1]);
            }

            void HandleWindowClosed(object? sender, EventArgs e)
            {
                foreach (var disposable in this.Categories
                    .SelectMany(x => x.Contents)
                    .Select(x => x.Instance)
                    .OfType<FrameworkElement>()
                    .Select(x => x.DataContext)
                    .OfType<IDisposable>())
                {
                    disposable?.Dispose();
                }
            }
        }

        public IEnumerable<MainContent<T>> EnumerateContents<T>()
            => this.Categories
                .SelectMany(x => x.Contents)
                .Where(x => x.Instance is FrameworkElement { DataContext: T _ })
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
