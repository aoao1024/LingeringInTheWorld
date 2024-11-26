using System;
using System.Globalization;
using Avalonia.Data.Converters;
using LingeringInTheWorld.Library.Models;

namespace LingeringInTheWorld.Converters;

public class DiaryToStringConverter : IValueConverter
{
    // 这个是接口的 Convert 方法实现，必须实现
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // 确保值为 Diary 类型
        if (value is Diary diary)
        {
            // 判断传入的参数是否为 "details" 或 "preview"，选择不同的转换方法
            if (parameter is string a && a == "details")
            {
                return ConvertForDetails(diary);
            }
            else if (parameter is string b && b == "snippet")
            {
                return ConvertForSnippet(diary);
            }
            else
            {
                return ConvertForPreview(diary);
            }
        }
        return null;
    }

    // 方法1: 根据 Location、Tags 拼接字符串
    //<TextBlock Text="{Binding Diary, Converter={StaticResource DiaryToStringConverter}, ConverterParameter=details}" />
    private object ConvertForDetails(Diary diary)
    {
        // 如果 Location 字段不为空，则添加"位置: <Location>"，否则不添加
        var location = !string.IsNullOrEmpty(diary.Location) ? $"📍 {diary.Location}" : string.Empty;
        
        // 如果 Tags 字段不为空，则添加"标签: <Tags>"，否则不添加
        var tags = !string.IsNullOrEmpty(diary.Tags) ? $"🏷 {diary.Tags}" : string.Empty;
        
        // 使用空格或者逗号连接所有非空的字段
        var result = $"{location}   {tags}".Trim();

        return result;
    }
    
    // 方法2: 根据 Snippet 拼接字符串
    private object ConvertForSnippet(Diary diary)
    {
        // 如果 Snippet 字段不为空，则添加"预览: <Snippet>"，否则不添加
        var snippet = !string.IsNullOrEmpty(diary.Snippet) ? $" {diary.Snippet}" : string.Empty;

        // 使用空格或者逗号连接所有非空的字段
        var result = $"{snippet}".Trim();

        return result;
    }

    // 方法3: 根据 DateTime、Weather 和 Title 格式化输出
    //<TextBlock Text="{Binding Diary, Converter={StaticResource DiaryToStringConverter}, ConverterParameter=preview}" />
    private object ConvertForPreview(Diary diary)
    {
        // 格式化输出： 创建时间 · 天气 · 标题
        string formattedDate = diary.DateTime.ToString("yyyy-MM-dd");
        return $"{formattedDate}  ·  {diary.Weather}  ·  {diary.Title}";
    }

    // IValueConverter 的 ConvertBack 方法通常是不能使用的，抛出异常
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }
}
