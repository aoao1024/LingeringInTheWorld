using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace LingeringInTheWorld.Converters;

public class StatusToBackGroundConverter:IMultiValueConverter
{
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        bool status = (bool)values[0]!;
        DateTime date = (DateTime)values[1]!;
        if (status)
        {
            return Brushes.Azure;
        }
        if (!status&&date<DateTime.Now)
        {
            return Brushes.Gainsboro;
        }
        return Brushes.White;
    }
}