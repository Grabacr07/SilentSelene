using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Livet;
using MetroTrilithon.Serialization;

namespace MetroTrilithon.Mvvm
{
    public static class DisplayViewModel
    {
        public static DisplayViewModel<T> Create<T>(T value, string display)
            => new DisplayViewModel<T>
            {
                Value = value,
                Display = display,
            };

        public static DisplayViewModel<T> ToDefaultDisplay<T>(this SerializableProperty<T> property, string display)
            => new DisplayViewModel<T>
            {
                Value = property.Default,
                Display = display,
            };

        public static IEnumerable<DisplayViewModel<TResult>> ToDisplay<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> valueSelector, Func<TSource, string> displaySelector)
            => source.Select(x => new DisplayViewModel<TResult>
            {
                Value = valueSelector(x),
                Display = displaySelector(x),
            });
    }

    public interface IHaveDisplayName
    {
        string Display { get; }
    }

    public class DisplayViewModel<T> : ViewModel, IHaveDisplayName
    {
        #region Value 変更通知プロパティ

        [AllowNull]
        private T _Value = default!;

        [AllowNull]
        public T Value
        {
            get => this._Value;
            set
            {
                if (!Equals(this._Value, value))
                {
                    this._Value = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Display 変更通知プロパティ

        private string _Display = default!;

        public string Display
        {
            get => this._Display;
            set
            {
                if (this._Display != value)
                {
                    this._Display = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        public override string? ToString()
            => this.Display;
    }
}
