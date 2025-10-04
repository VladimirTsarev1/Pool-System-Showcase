using System;
using System.Globalization;

namespace Showcase.Scripts.Core.Extensions
{
    public static class FloatExtensions
    {
        public static string OneDecimalToString(this float value)
        {
            float roundedValue = (float)(Math.Truncate(value * 10f) / 10f);
            return roundedValue.ToString("0.0", CultureInfo.InvariantCulture);
        }
    }
}