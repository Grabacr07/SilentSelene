using System;
using System.Diagnostics;

namespace WindowsDesktopApp.UI.Hosting
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class MainContentAttribute : Attribute
    {
        public string? Header { get; }

        public string? HeaderIconAsset { get; }

        public string Category { get; }

        public int Order { get; }

        public MainContentAttribute(string category, int order)
        {
            this.Category = category;
            this.Order = order;
        }

        public MainContentAttribute(string header, string headerIconAsset, string category, int order)
        {
            this.Header = header;
            this.HeaderIconAsset = headerIconAsset;
            this.Category = category;
            this.Order = order;
        }
    }

    /// <remarks>
    /// リリース ビルドで公開しない機能向け。
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false), Conditional("DEBUG")]
    public class InternalContentAttribute : MainContentAttribute
    {
        public InternalContentAttribute(string header, string headerIconAsset, string category, int order)
            : base(header, headerIconAsset, category, order)
        {
        }
    }
}
