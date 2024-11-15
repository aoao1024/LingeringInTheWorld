using LingeringInTheWorld.Library.Models;

namespace LingeringInTheWorld.Library.Services;

public interface IWeatherService
{
    /// <summary>
    /// 根据经纬度获取实时天气信息。
    /// </summary>
    /// <param name="latitude">纬度</param>
    /// <param name="longitude">经度</param>
    /// <returns>天气信息</returns>
    Task<WeatherInfo> GetWeatherByLocationAsync(double latitude, double longitude);
}
