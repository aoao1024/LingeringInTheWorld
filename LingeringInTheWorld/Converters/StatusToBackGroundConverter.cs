using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace LingeringInTheWorld.Converters;

public class StatusToBackGroundConverter:IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool status)
        {
            return status ? Brushes.Azure : Brushes.White;
        }
        return Brushes.White; // 默认背景色
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}