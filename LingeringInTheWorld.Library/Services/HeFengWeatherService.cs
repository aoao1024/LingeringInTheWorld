using System.IO.Compression;
using System.Text;
using System.Text.Json;
using LingeringInTheWorld.Library.Helpers;
using LingeringInTheWorld.Library.Models;

namespace LingeringInTheWorld.Library.Services;
public class HeFengWeatherService : IWeatherService
{
    private readonly IAlertService _alertService;
    private const string Server = "和风天气服务器";

    // HttpClient 可以作为静态字段，提高性能
    private static readonly HttpClient _httpClient = new ();
    
    // 可以将 API Key 放在环境变量中，避免硬编码
    private const string ApiKey = "551022de4c4541f1b66014aa55cab174";  // 替换为你的 API Key
    private const string ApiUrl = "https://devapi.qweather.com/v7/weather/now"; // 实时天气查询接口
    private const string GeoApiUrl = "https://geoapi.qweather.com/v2/city/lookup"; // 城市查询接口

    public HeFengWeatherService(IAlertService alertService)
    {
        _alertService = alertService;
    }

    // 异步获取天气信息
   public async Task<WeatherInfo> GetWeatherByLocationAsync(double latitude, double longitude) 
   {
        var requestUrl = $"{ApiUrl}?location={longitude},{latitude}&key={ApiKey}";

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("Accept-Encoding", "gzip, deflate");

            // 发送请求并获取响应
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode(); // 如果响应不成功，会抛出异常

            // 获取响应内容流
            var stream = await response.Content.ReadAsStreamAsync();

            // 获取字符编码，默认UTF-8
            var encoding = Encoding.UTF8;
            if (response.Content.Headers.ContentType?.CharSet != null)
            {
                try
                {
                    encoding = Encoding.GetEncoding(response.Content.Headers.ContentType.CharSet);
                }
                catch (ArgumentException)
                {
                    encoding = Encoding.UTF8; // 如果无法识别编码，则使用 UTF-8
                }
            }

            // 如果内容是 Gzip 压缩格式，解压缩
            if (response.Content.Headers.ContentEncoding.Contains("gzip"))
            {
                using (var gzipStream = new GZipStream(stream, CompressionMode.Decompress))
                using (var reader = new StreamReader(gzipStream, encoding))
                {
                    var json = await reader.ReadToEndAsync();
                    return await ParseWeatherJson(json);
                }
            }
            else
            {
                // 非 Gzip 压缩，直接读取内容
                using (var reader = new StreamReader(stream, encoding))
                {
                    var json = await reader.ReadToEndAsync();
                    return await ParseWeatherJson(json);
                }
            }
        }
        catch (HttpRequestException e)
        {
            // 捕获HTTP请求异常
            await _alertService.AlertAsync(
                ErrorMessageHelper.HttpClientErrorTitle,
                ErrorMessageHelper.GetHttpClientError(Server, e.Message));
        }
        catch (Exception e)
        {
            // 捕获其他类型的异常（如解析错误等）
            await _alertService.AlertAsync(
                ErrorMessageHelper.HttpClientErrorTitle,
                ErrorMessageHelper.GetHttpClientError(Server, e.Message));
        }

        // 如果发生异常或者未能获取到数据，返回null
        return null;
    }


    private async Task<WeatherInfo> ParseWeatherJson(string json)
    {
        try
        {
            HeFengWeatherResponse weatherData = JsonSerializer.Deserialize<HeFengWeatherResponse>(json);
            if (weatherData?.code == "200")
            {
                var weather = weatherData?.now;
                if (weather != null)
                {
                    return new WeatherInfo
                    {
                        Temperature = weather.temp,
                        Condition = weather.text,
                        Wind = weather.windDir
                    };
                }
            }
        }
        catch (Exception e)
        {
            // 解析 JSON 时出现错误
            await _alertService.AlertAsync(
                ErrorMessageHelper.JsonDeserializationErrorTitle,
                ErrorMessageHelper.GetJsonDeserializationError(Server, e.Message));
        }

        return null;
    }
    
    // 实现根据城市名称获取经纬度
    public async Task<(double Latitude, double Longitude)> GetCoordinatesFromLocation(string location)
    {
        var requestUrl = $"{GeoApiUrl}?key={ApiKey}&location={location}";

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("Accept-Encoding", "gzip, deflate");

            // 发送请求
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode(); // 确保请求成功

            var stream = await response.Content.ReadAsStreamAsync();

            // 获取响应的字符编码，如果没有指定，则使用 UTF-8
            var encoding = Encoding.UTF8;
            if (response.Content.Headers.ContentType?.CharSet != null)
            {
                try
                {
                    encoding = Encoding.GetEncoding(response.Content.Headers.ContentType.CharSet);
                }
                catch (ArgumentException)
                {
                    encoding = Encoding.UTF8; // 如果无法识别编码，则默认使用 UTF-8
                }
            }

            // 判断是否为 Gzip 压缩
            if (response.Content.Headers.ContentEncoding.Contains("gzip"))
            {
                // Gzip 解压缩
                using (var gzipStream = new GZipStream(stream, CompressionMode.Decompress))
                using (var reader = new StreamReader(gzipStream, encoding))
                {
                    var json = await reader.ReadToEndAsync();
                    return await ParseCoordinatesJson(json);
                }
            }
            else
            {
                // 非 Gzip 压缩，直接读取
                using (var reader = new StreamReader(stream, encoding))
                {
                    var json = await reader.ReadToEndAsync();
                    return await ParseCoordinatesJson(json);
                }
            }
        }
        catch (Exception e)
        {
            // 捕获任何异常，可能是网络请求失败，解析失败等
            await _alertService.AlertAsync(
                ErrorMessageHelper.HttpClientErrorTitle,
                ErrorMessageHelper.GetHttpClientError(Server, e.Message));
        }

        // 如果无法获取经纬度，返回 (0.0, 0.0)
        return (0.0, 0.0);
    }

    // 解析城市经纬度的 JSON 数据
    private async Task<(double Latitude, double Longitude)> ParseCoordinatesJson(string json)
    {
        try
        {
            var jsonResponse = JsonDocument.Parse(json);
            var cityInfo = jsonResponse.RootElement
                                        .GetProperty("location")
                                        .EnumerateArray()
                                        .FirstOrDefault();
            if (cityInfo.ValueKind == JsonValueKind.Object)
            {
                // 使用 GetString 获取 lat 和 lon，并使用 double.TryParse 转换为数字类型
                var latitudeString = cityInfo.GetProperty("lat").GetString();
                var longitudeString = cityInfo.GetProperty("lon").GetString();

                // 尝试将字符串转换为 double
                if (double.TryParse(latitudeString, out double latitude) &&
                    double.TryParse(longitudeString, out double longitude))
                {
                    return (latitude, longitude); // 返回经纬度
                }
                else
                {
                    // 如果转换失败，抛出异常或者返回默认值
                    await _alertService.AlertAsync(Server,"无法将经纬度字符串转换为数字");
                }
            }
        }
        catch (Exception e)
        {
            // 解析 JSON 时出现错误
            await _alertService.AlertAsync(
                ErrorMessageHelper.JsonDeserializationErrorTitle,
                ErrorMessageHelper.GetJsonDeserializationError(Server, e.Message));
        }

        // 如果无法解析或获取经纬度，返回 (0.0,0.0)
        return (0.0,0.0);
    }

}

public class HeFengWeatherResponse
{
    public string code { get; set; }  // 响应码
    public string updateTime { get; set; }  // 更新时间
    public string fxLink { get; set; }  // 链接
    public HeFengWeatherNow now { get; set; }  // 当前天气数据
    public HeFengWeatherRefer refer { get; set; }  // 来源信息
}

public class HeFengWeatherNow
{
    public string obsTime { get; set; }  // 观测时间
    public string temp { get; set; }  // 当前温度
    public string feelsLike { get; set; }  // 体感温度
    public string icon { get; set; }  // 天气图标
    public string text { get; set; }  // 天气描述
    public string wind360 { get; set; }  // 风向角度
    public string windDir { get; set; }  // 风向
    public string windScale { get; set; }  // 风力等级
    public string windSpeed { get; set; }  // 风速
    public string humidity { get; set; }  // 湿度
    public string precip { get; set; }  // 降水量
    public string pressure { get; set; }  // 气压
    public string vis { get; set; }  // 可见度
    public string cloud { get; set; }  // 云量
    public string dew { get; set; }  // 露点
}

public class HeFengWeatherRefer
{
    public List<string> sources { get; set; }  // 数据来源
    public List<string> license { get; set; }  // 授权信息
}
