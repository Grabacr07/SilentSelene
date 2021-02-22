using System;
using System.Collections.Generic;
using System.Text;

namespace ResinTimer.Functions
{
    public static class DateTimeUtil
    {
        public static DateTimeOffset EnsureRange(this DateTimeOffset value, DateTimeOffset min, DateTimeOffset max)
            => value < min
                ? min
                : max < value
                    ? max
                    : value;
    }
}
