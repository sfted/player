using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls.Primitives;
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

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class PlayerIsTrackSetVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    [ValueConversion(typeof(Player.PlaybackModes), typeof(Geometry))]
    public class PlaybackModesToIconsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((Player.PlaybackModes)value)
            {
                case Player.PlaybackModes.Default:
                    return Application.Current.Resources["icon-playback-default"];
                case Player.PlaybackModes.RepeatAll:
                    return Application.Current.Resources["icon-repeat-all"];
                case Player.PlaybackModes.RepeatOne:
                    return Application.Current.Resources["icon-repeat-once"];
                case Player.PlaybackModes.Shuffle:
                    return Application.Current.Resources["icon-shuffle"];
                default:
                    return Application.Current.Resources["icon-playback-default"];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
