using ResinTimer.UI.Controls;

namespace ResinTimer.UI.Hosting
{
    public interface IContentHeaderProvider
    {
        object GetHeader();

        public class Default : IContentHeaderProvider
        {
            private readonly string _header;
            private readonly string _iconAsset;

            public Default(string header, string iconAsset)
            {
                this._header = header;
                this._iconAsset = iconAsset;
            }

            public object GetHeader()
                => new MainContentHeader()
                {
                    IconAsset = this._iconAsset,
                    Content = this._header,
                };
        }
    }
}
