using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Player.Core.Utils
{
    [ValueConversion(typeof(System.Drawing.Color), typeof(Brush))]
    public class DrawingColorToBrushConverter : IValueConverter
    {
        private static Color WHITE = Color.FromRgb(255, 255, 255);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Drawing.Color color)
                return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
            else
                return new SolidColorBrush(WHITE);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
