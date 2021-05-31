using Player.Core.Entities;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Player.Core.Utils
{
    [ValueConversion(typeof(object[]), typeof(object[]))]
    public class UselessButNeededConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(bool), typeof(Geometry))]
    public class IsPlayingToPlayPauseIconsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isPlaying = (bool)value;
            if (isPlaying)
                return Application.Current.Resources["icon-pause"];
            else
                return Application.Current.Resources["icon-play"];

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(PlaybackQueue), typeof(Visibility))]
    public class QueueToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((PlaybackQueue)value != null)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(PlaybackQueue.RepeatModes), typeof(Geometry))]
    public class RepeatModesToIconsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((PlaybackQueue.RepeatModes)value)
            {
                case PlaybackQueue.RepeatModes.NONE:
                    return Application.Current.Resources["icon-repeat-all"];
                case PlaybackQueue.RepeatModes.REPEAT_ALL:
                    return Application.Current.Resources["icon-repeat-all"];
                case PlaybackQueue.RepeatModes.REPEAT_ONE:
                    return Application.Current.Resources["icon-repeat-once"];
                default:
                    return Application.Current.Resources["icon-repeat-all"];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(PlaybackQueue.RepeatModes), typeof(SolidColorBrush))]
    public class RepeatModesToBrushConverter : IValueConverter
    {
        private static readonly SolidColorBrush blue = new(Colors.DodgerBlue);
        private static readonly SolidColorBrush gray = new(Colors.Gray);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((PlaybackQueue.RepeatModes)value)
            {
                case PlaybackQueue.RepeatModes.NONE:
                    return gray;
                case PlaybackQueue.RepeatModes.REPEAT_ALL:
                    return blue;
                case PlaybackQueue.RepeatModes.REPEAT_ONE:
                    return blue;
                default:
                    return gray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
    public class ShuffleToBrushConverter : IValueConverter
    {
        private static readonly SolidColorBrush blue = new(Colors.DodgerBlue);
        private static readonly SolidColorBrush gray = new(Colors.Gray);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return blue;
            else
                return gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
