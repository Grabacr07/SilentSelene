using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using MetroRadiance.Internal;
using MetroRadiance.Media;
using MetroRadiance.Platform;
using UriTemplateEx = UriTemplate.Core.UriTemplate;

namespace MetroRadiance.UI
{
    /// <summary>
    /// MetroRadiance テーマ機能を提供します。
    /// </summary>
    public class ThemeService : INotifyPropertyChanged
    {
        #region singleton members

        public static ThemeService Current { get; } = new ThemeService();

        #endregion

        private static readonly UriTemplateEx _themeTemplate = new UriTemplateEx(@"Themes/{theme}.xaml");
        private static readonly UriTemplateEx _accentTemplate = new UriTemplateEx(@"Themes/Accents/{accent}.xaml");
        private static readonly Uri _templateBaseUri = new Uri(@"pack://application:,,,/MetroRadiance;component");

        private Dispatcher? _dispatcher;
        private IDisposable? _windowsAccentListener;
        private IDisposable? _windowsThemeListener;

        private readonly List<ResourceDictionary> _themeResources = new List<ResourceDictionary>();
        private readonly List<ResourceDictionary> _accentResources = new List<ResourceDictionary>();

        #region Theme 変更通知プロパティ

        private Theme? _Theme;

        /// <summary>
        /// 現在設定されているテーマを取得します。
        /// </summary>
        public Theme Theme
        {
            get => this._Theme ?? Theme.Windows;
            private set
            {
                if (this._Theme != value)
                {
                    this._Theme = value;
                    this.UpdateListener(value);
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Accent 変更通知プロパティ

        private Accent? _Accent;

        /// <summary>
        /// 現在設定されているアクセントを取得します。
        /// </summary>
        public Accent Accent
        {
            get => this._Accent ?? Accent.Windows;
            private set
            {
                if (this._Accent != value)
                {
                    this._Accent = value;
                    this.UpdateListener(value);
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        private ThemeService()
        {
        }

        /// <summary>
        /// テーマ機能を有効化します。テーマまたはアクセントが変更されたとき、<paramref name="app"/>
        /// で指定した WPF アプリケーション内のテーマ関連リソースは自動的に書き換えられます。
        /// </summary>
        /// <param name="app">テーマ関連リソースを含む WPF アプリケーション。</param>
        /// <param name="theme">初期値として使用するテーマ。</param>
        /// <param name="accent">初期値として使用するアクセント。</param>
        /// <returns><paramref name="app"/> をリソースの書き換え対象から外すときに使用する <see cref="IDisposable"/> オブジェクト。</returns>
        public IDisposable Register(Application app, Theme theme, Accent accent)
        {
            this._dispatcher = app.Dispatcher;

            var disposable = this.Register(app.Resources, theme, accent);

            this.Theme = theme;
            this.Accent = accent;

            return disposable;
        }

        /// <summary>
        /// テーマまたはアクセントが変更されたときにリソースの書き換え対象とする <see cref="ResourceDictionary"/>
        /// を登録します。このメソッドは、登録解除に使用する <see cref="IDisposable"/> オブジェクトを返します。
        /// </summary>
        /// <returns><paramref name="rd"/> をリソースの書き換え対象から外すときに使用する <see cref="IDisposable"/> オブジェクト。</returns>
        public IDisposable Register(ResourceDictionary rd)
        {
            return this.Register(rd, this.Theme, this.Accent);
        }

        internal IDisposable Register(ResourceDictionary rd, Theme theme, Accent accent)
        {
            var allDictionaries = EnumerateDictionaries(rd).ToArray();

            var themeDic = GetThemeResource(theme);
            var targetThemeDic = allDictionaries.FirstOrDefault(x => CheckThemeResourceUri(x.Source));
            if (targetThemeDic == null)
            {
                targetThemeDic = themeDic;
                rd.MergedDictionaries.Add(targetThemeDic);
            }
            else
            {
                foreach (var key in themeDic.Keys.OfType<string>().Where(x => targetThemeDic.Contains(x)))
                {
                    targetThemeDic[key] = themeDic[key];
                }
            }

            this._themeResources.Add(targetThemeDic);

            var accentDic = GetAccentResource(accent);
            var targetAccentDic = allDictionaries.FirstOrDefault(x => CheckAccentResourceUri(x.Source));
            if (targetAccentDic == null)
            {
                targetAccentDic = accentDic;
                rd.MergedDictionaries.Add(targetAccentDic);
            }
            else
            {
                foreach (var key in accentDic.Keys.OfType<string>().Where(x => targetAccentDic.Contains(x)))
                {
                    targetAccentDic[key] = accentDic[key];
                }
            }

            this._accentResources.Add(targetAccentDic);

            // Unregister したいときは戻り値の IDisposable を Dispose() してほしい
            return Disposable.Create(() =>
            {
                this._themeResources.Remove(targetThemeDic);
                this._accentResources.Remove(targetAccentDic);
            });
        }

        public void ChangeTheme(Theme theme)
        {
            if (this.Theme == theme) return;

            this.InvokeOnUIDispatcher(() => this.ChangeThemeCore(theme));
            this.Theme = theme;
        }

        private void ChangeThemeCore(Platform.Theme theme)
        {
            switch (theme)
            {
                case Platform.Theme.Dark:
                    this.ChangeThemeCore(Theme.Dark);
                    break;
                case Platform.Theme.Light:
                    this.ChangeThemeCore(Theme.Light);
                    break;
            }
        }

        private void ChangeThemeCore(Theme theme)
        {
            var dic = GetThemeResource(theme);

            foreach (var key in dic.Keys.OfType<string>())
            {
                foreach (var resource in this._themeResources.Where(x => x.Contains(key)))
                {
                    resource[key] = dic[key];
                }
            }
        }

        public void ChangeAccent(Accent accent)
        {
            if (this.Accent == accent) return;

            this.InvokeOnUIDispatcher(() => this.ChangeAccentCore(accent));
            this.Accent = accent;
        }

        private void ChangeAccentCore(Accent accent)
        {
            this.ChangeAccentCore(GetAccentResource(accent));
        }

        private void ChangeAccentCore(Color color)
        {
            this.ChangeAccentCore(GetAccentResource(color));
        }

        private void ChangeAccentCore(ResourceDictionary dic)
        {
            foreach (var key in dic.Keys.OfType<string>())
            {
                foreach (var resource in this._accentResources.Where(x => x.Contains(key)))
                {
                    resource[key] = dic[key];
                }
            }
        }

        private static ResourceDictionary GetThemeResource(Theme theme)
        {
            var specified = theme.SyncToWindows
                ? WindowsTheme.Theme.Current == Platform.Theme.Dark ? Theme.Dark.Specified : Theme.Light.Specified
                : theme.Specified;
            if (specified == null) throw new ArgumentException($"Invalid theme value '{theme}'.");

            return new ResourceDictionary
            {
                Source = CreateThemeResourceUri(specified.Value),
            };
        }

        private static ResourceDictionary GetAccentResource(Accent accent)
        {
            return accent.Specified != null
                ? new ResourceDictionary
                {
                    Source = CreateAccentResourceUri(accent.Specified.Value),
                }
                : GetAccentResource(accent.Color ?? WindowsTheme.Accent.Current);
        }

        private static ResourceDictionary GetAccentResource(Color color)
        {
            // Windows のテーマがアルファ チャネル 255 以外の色を返してくるけど、
            // HSV で Active と Highlight 用の色を作る過程で結局失われるので、
            // アルファ チャネルは 255 しかサポートしないようにしてしまおう感。
            color.A = 255;

            var hsv = color.ToHsv();
            var dark = HsvColor.FromHsv(hsv.H, hsv.S, hsv.V * 0.8);
            var light = HsvColor.FromHsv(hsv.H, hsv.S * 0.6, hsv.V);

            var activeColor = dark.ToRgb();
            var highlightColor = light.ToRgb();

            var luminosity = Luminosity.FromRgb(color);
            var foreground = luminosity < 128 ? Colors.White : Colors.Black;

            var dic = new ResourceDictionary
            {
                ["AccentColorKey"] = color,
                ["AccentBrushKey"] = new SolidColorBrush(color),
                ["AccentActiveColorKey"] = activeColor,
                ["AccentActiveBrushKey"] = new SolidColorBrush(activeColor),
                ["AccentHighlightColorKey"] = highlightColor,
                ["AccentHighlightBrushKey"] = new SolidColorBrush(highlightColor),
                ["AccentForegroundColorKey"] = foreground,
                ["AccentForegroundBrushKey"] = new SolidColorBrush(foreground),
            };

            return dic;
        }

        private void UpdateListener(Accent accent)
        {
            if (accent == Accent.Windows)
            {
                if (this._windowsAccentListener == null)
                {
                    // アクセントが Windows 依存で、リスナーが未登録だったら購読する
                    this._windowsAccentListener = WindowsTheme.Accent.RegisterListener(this.ChangeAccentCore);
                }
            }
            else if (this._windowsAccentListener != null)
            {
                // アクセントが Windows 依存でないのにリスナーが登録されてたら解除する
                this._windowsAccentListener.Dispose();
                this._windowsAccentListener = null;
            }
        }

        private void UpdateListener(Theme theme)
        {
            if (theme == Theme.Windows)
            {
                this._windowsThemeListener ??= WindowsTheme.Theme.RegisterListener(this.ChangeThemeCore);
            }
            else if (this._windowsThemeListener != null)
            {
                this._windowsThemeListener.Dispose();
                this._windowsThemeListener = null;
            }
        }

        /// <summary>
        /// 指定した <see cref="Uri"/> がテーマのリソースを指す URI かどうかをチェックします。
        /// </summary>
        /// <returns><paramref name="uri"/> がテーマのリソースを指す URI の場合は true、それ以外の場合は false。</returns>
        private static bool CheckThemeResourceUri(Uri uri)
        {
            return _themeTemplate.Match(_templateBaseUri, uri) != null;
        }

        /// <summary>
        /// 指定した <see cref="Uri"/> がアクセント カラーのリソースを指す URI かどうかをチェックします。
        /// </summary>
        /// <returns><paramref name="uri"/> がアクセント カラーのリソースを指す URI の場合は true、それ以外の場合は false。</returns>
        private static bool CheckAccentResourceUri(Uri uri)
        {
            return _accentTemplate.Match(_templateBaseUri, uri) != null;
        }

        private static Uri CreateThemeResourceUri(Theme.SpecifiedColor theme)
        {
            var param = new Dictionary<string, string>
            {
                { "theme", theme.ToString() },
            };
            return _themeTemplate.BindByName(_templateBaseUri, param);
        }

        private static Uri CreateAccentResourceUri(Accent.SpecifiedColor accent)
        {
            var param = new Dictionary<string, string>
            {
                { "accent", accent.ToString() },
            };
            return _accentTemplate.BindByName(_templateBaseUri, param);
        }

        private static IEnumerable<ResourceDictionary> EnumerateDictionaries(ResourceDictionary dictionary)
        {
            if (dictionary.MergedDictionaries.Count == 0)
            {
                yield break;
            }

            foreach (var mergedDictionary in dictionary.MergedDictionaries)
            {
                yield return mergedDictionary;

                foreach (var other in EnumerateDictionaries(mergedDictionary))
                {
                    yield return other;
                }
            }
        }

        private void InvokeOnUIDispatcher(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            (this._dispatcher ?? Application.Current.Dispatcher).BeginInvoke(action, priority);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
