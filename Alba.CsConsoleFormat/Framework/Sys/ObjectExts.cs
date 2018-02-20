﻿using System.Collections.Generic;
using JetBrains.Annotations;

namespace Alba.CsConsoleFormat.Framework.Sys
{
    internal static class ObjectExts
    {
        public static bool EqualsValue<T>([CanBeNull] this T @this, [CanBeNull] T value) => EqualityComparer<T>.Default.Equals(@this, value);
    }
}