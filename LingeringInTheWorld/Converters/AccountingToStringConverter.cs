using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace LingeringInTheWorld.Converters;

public class AccountingToStringConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter,
        CultureInfo culture) =>
        value is Accounting accounting
            ? $"{accounting.Type} · {accounting.Category}   {accounting.Content}   {accounting.Amount}"
            : null;

    public object ConvertBack(object value, Type targetType, object parameter,
        CultureInfo culture) =>
        throw new InvalidOperationException();
}