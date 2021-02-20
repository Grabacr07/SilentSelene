using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ResinTimer.UI.Hosting
{
    public class MainContentCategory : Control
    {
        static MainContentCategory()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(MainContentCategory),
                new FrameworkPropertyMetadata(typeof(MainContentCategory)));
        }

        private readonly MainContentHost _host;

        #region CategoryName readonly dependency property

        private static readonly DependencyPropertyKey CategoryNamePropertyKey
            = DependencyProperty.RegisterReadOnly(
                nameof(CategoryName),
                typeof(string),
                typeof(MainContentCategory),
                new PropertyMetadata(default(string)));

        public static readonly DependencyProperty CategoryNameProperty
            = CategoryNamePropertyKey.DependencyProperty;

        public string CategoryName
        {
            get => (string)this.GetValue(CategoryNameProperty);
            private set => this.SetValue(CategoryNamePropertyKey, value);
        }

        #endregion

        #region Contents readonly dependency property

        private static readonly DependencyPropertyKey ContentsPropertyKey
            = DependencyProperty.RegisterReadOnly(
                nameof(Contents),
                typeof(IReadOnlyList<MainContent>),
                typeof(MainContentCategory),
                new PropertyMetadata(default(IReadOnlyList<MainContent>)));

        public static readonly DependencyProperty ContentsProperty
            = ContentsPropertyKey.DependencyProperty;

        public IReadOnlyList<MainContent> Contents
        {
            get => (IReadOnlyList<MainContent>)this.GetValue(ContentsProperty);
            private set => this.SetValue(ContentsPropertyKey, value);
        }

        #endregion

        #region SelectedContent dependency property

        public static readonly DependencyProperty SelectedContentProperty
            = DependencyProperty.Register(
                nameof(SelectedContent),
                typeof(MainContent),
                typeof(MainContentCategory),
                new PropertyMetadata(default(MainContent), HandleSelectedContentPropertyChanged));

        private static void HandleSelectedContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is MainContentCategory category)) return;

            var value = (MainContent?)e.NewValue;
            if (value == null) return;

            // SelectedItem に null 突っ込むと選択解除される ListBox の仕様を利用して、
            // MainContentHost の Current 依存関係プロパティとのバインディングによって
            // 複数の ListBox で SelectedItem のバインディング ソースを共用する
            if (category.Contents.Any(x => x == value))
            {
                category._host.Current = value;
            }
            else
            {
                category.SetValue(SelectedContentProperty, null);
            }
        }

        public MainContent? SelectedContent
        {
            get => (MainContent?)this.GetValue(SelectedContentProperty);
            set => this.SetValue(SelectedContentProperty, value);
        }

        #endregion

        internal MainContentCategory(MainContentHost host, string categoryName, IEnumerable<MainContent> contents)
        {
            this._host = host;
            this.CategoryName = categoryName;
            this.Contents = new List<MainContent>(contents);
        }
    }
}
