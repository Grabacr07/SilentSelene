using System;
using System.Linq;

namespace ResinTimer.UI.Hosting
{
    public class MainContent
    {
        public static class Categories
        {
            public const string Feature = "1000:メイン機能";
            public const string DevTool = "8000:開発ツール";
            public const string Option = "9000:オプション";
        }

        private readonly Lazy<object> _instance;
        private readonly Action<MainContent> _activateHandler;

        public object Instance
            => this._instance.Value;

        public object Header { get; }

        public string Category { get; }

        public int Order { get; }

        public MainContent(Type contentType, MainContentAttribute attribute, Action<MainContent> activateHandler)
        {
            var hasHeaderProvider = contentType.GetInterfaces().Any(x => x == typeof(IContentHeaderProvider));
            if (hasHeaderProvider == false && string.IsNullOrEmpty(attribute.Header)) throw new InvalidOperationException($"Header is missing in type '{contentType}'.");

            this._activateHandler = activateHandler;
            this._instance = new Lazy<object>(() =>
            {
                try
                {
                    return Activator.CreateInstance(contentType) ?? $"Failed to instantiate type '{contentType}'.";
                }
                catch (Exception ex)
                {
                    Application.Current.Tracker.TrackException(this, ex);
                    return ex;
                }
            });
            this.Header = (hasHeaderProvider
                ? (IContentHeaderProvider)this._instance.Value
                : new IContentHeaderProvider.Default(attribute.Header ?? "", attribute.HeaderIconAsset ?? "")).GetHeader();
            this.Category = attribute.Category;
            this.Order = attribute.Order;
        }

        public void Activate()
            => this._activateHandler(this);
    }

    public class MainContent<T>
    {
        private readonly MainContent _source;

        public T DataContext { get; }

        public MainContent(MainContent source, T dataContext)
        {
            this._source = source;
            this.DataContext = dataContext;
        }

        public void Activate()
            => this._source.Activate();
    }
}
