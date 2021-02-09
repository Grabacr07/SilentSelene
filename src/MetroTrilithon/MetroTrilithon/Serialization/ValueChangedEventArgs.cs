using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MetroTrilithon.Serialization
{
    public class ValueChangedEventArgs<T> : EventArgs
    {
        [AllowNull]
        public T OldValue { get; }

        [AllowNull]
        public T NewValue { get; }

        public ValueChangedEventArgs([AllowNull] T oldValue, [AllowNull] T newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }
    }
}
