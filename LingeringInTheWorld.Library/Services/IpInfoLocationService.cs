using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace LingeringInTheWorld.Library.Services;
public class IpInfoLocationService : ILocationService
{
    private readonly IAlertService _alertService;
    private const string Server = "IP定位服务";

    private const string IpInfoApiUrl = "http://ipinfo.io";  // ipinfo.io 提供的公共 API

    public IpInfoLocationService(IAlertService alertService)
    {
        _alertService = alertService;
    }

    public async Task<(double Latitude, double Longitude, string LocationName)> GetCurrentLocationAsync()
    {
        try
        {
            using var httpClient = new HttpClient();
            
            // 获取外部服务返回的 IP 地址
            var ipResponse = await httpClient.GetStringAsync("http://checkip.amazonaws.com");
            var ip = ipResponse.Trim();
            
            // 请求 ipinfo.io 获取基于 IP 的位置信息
            var requestUrl = $"{IpInfoApiUrl}/{ip}/json";
            var response = await httpClient.GetStringAsync(requestUrl);
            var locationData = JsonSerializer.Deserialize<IpInfoLocationResponse>(response);

            if (locationData != null && locationData.loc != null)
            {
                var coords = locationData.loc.Split(',');
                double latitude = double.Parse(coords[0]);
                double longitude = double.Parse(coords[1]);

                
                return (latitude, longitude, locationData.city);
            }
            else
            {
                await _alertService.AlertAsync(Server, "无法获取到地理位置，请检查 API 设置或网络连接。");
            }
        }
        catch (Exception ex)
        {
            await _alertService.AlertAsync(Server, $"获取地理位置时发生错误: {ex.Message}");
        }

        return (41.48, 123.25, "沈阳"); // 返回默认值，表示定位失败
    }
}

// IPinfo API 返回的定位信息
public class IpInfoLocationResponse
{
    public string ip { get; set; } // IP 地址
    public string city { get; set; } // 城市
    public string region { get; set; } // 区域
    public string country { get; set; } // 国家
    public string loc { get; set; } // 位置信息，格式为 "latitude,longitude"
}

