﻿using System;
using System.Collections.Generic;

namespace SilentSelene.Utils;

internal static class Int32Util
{
    public static int EnsureRange(this int value, int max)
        => Math.Min(value, max);

    public static int EnsureRange(this int value, int min, int max)
        => Math.Max(min, Math.Min(value, max));
}
