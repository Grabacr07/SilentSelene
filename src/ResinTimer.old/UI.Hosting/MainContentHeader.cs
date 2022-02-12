using System.Windows;
using System.Windows.Controls;

namespace ResinTimer.UI.Hosting
{
    public class MainContentHeader : ContentControl
    {
        static MainContentHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MainContentHeader), new FrameworkPropertyMetadata(typeof(MainContentHeader)));
        }

        #region IconAsset dependency property

        public static readonly DependencyProperty IconAssetProperty
            = DependencyProperty.Register(
                nameof(IconAsset),
                typeof(string),
                typeof(MainContentHeader),
                new PropertyMetadata(default(string)));

        /// <summary>
        /// Segoe MDL2 Assets フォントで表示する Unicode ポイントを取得または設定します。
        /// </summary>
        /// <remarks>
        /// FYI: https://docs.microsoft.com/en-us/windows/uwp/design/style/segoe-ui-symbol-font
        /// </remarks>
        public string IconAsset
        {
            get => (string)this.GetValue(IconAssetProperty);
            set => this.SetValue(IconAssetProperty, value);
        }

        #endregion
    }
}
