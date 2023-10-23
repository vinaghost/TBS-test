using System;
using System.Globalization;
using System.Windows.Data;
using DColor = System.Drawing.Color;
using MColor = System.Windows.Media.Color;

namespace WPFUI.Converter
{
    [ValueConversion(typeof(DColor), typeof(MColor))]
    public class DrawingColorToMediaColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not DColor color) return null;

            return MColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not MColor color) return null;

            return DColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            if (fromType == typeof(DColor) && toType == typeof(MColor))
            {
                return 10;
            }
            return 0;
        }

        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            var color = (DColor)from;
            result = MColor.FromArgb(color.A, color.R, color.G, color.B);
            return true;
        }
    }
}