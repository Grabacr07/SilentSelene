using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JetBrains.Annotations;

namespace MetroRadiance.UI
{
    /// <summary>
    /// MetroRadiance テーマとして使用する色を識別します。
    /// </summary>
    [Serializable]
    public struct Theme : IEquatable<Theme>
    {
        /// <summary>
        /// テーマ専用の色が指定されている場合、その色を示す識別子を取得します。
        /// </summary>
        public SpecifiedColor? Specified { get; private set; }

        /// <summary>
        /// Windows のテーマと同期するかどうかを示す値を取得します。
        /// </summary>
        public bool SyncToWindows => this.Specified == null;

        private Theme(SpecifiedColor color) : this()
        {
            this.Specified = color;
        }

        public bool Equals(Theme theme)
        {
            return this == theme;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            return obj is Theme theme && this.Equals(theme);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return HashCode.Combine(this.Specified);
        }

        public override string ToString()
        {
            if (this.Specified != null) return this.Specified.Value.ToString();

            return "(Sync to Windows)";
        }

        public static bool operator ==(Theme theme1, Theme theme2)
        {
            return theme1.Specified == theme2.Specified;
        }

        public static bool operator !=(Theme theme1, Theme theme2)
        {
            return !(theme1 == theme2);
        }

        public static Theme Dark { get; }
            = new Theme(SpecifiedColor.Dark);

        public static Theme Light { get; }
            = new Theme(SpecifiedColor.Light);

        /// <summary>
        /// Windows のテーマ設定に追従するテーマを取得します。
        /// </summary>
        public static Theme Windows { get; }
            = new Theme();

        public enum SpecifiedColor
        {
            Dark,
            Light,
        }

        /// <summary>
        /// シリアル化インフラストラクチャをサポートします。コードから直接使用するためのものではありません。
        /// </summary>
        [UsedImplicitly]
        [EditorBrowsable(EditorBrowsableState.Never)]
        // ReSharper disable once InconsistentNaming
        public SpecifiedColor? __Specified
        {
            get => this.Specified;
            set => this.Specified = value;
        }
    }
}
