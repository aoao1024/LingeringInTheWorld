using LingeringInTheWorld.Library.Services;

namespace LingeringInTheWorld.Library.ViewModels;

public class DiaryAddViewModel : ViewModelBase
{
    private readonly IWeatherService _weatherService;
    
    private string _currentTime;    // 当前时间
    private string _currentWeatherCondition;    // 天气状况
    private string _currentLocation = "沈阳";    // 当前地址

    public DiaryAddViewModel(IWeatherService weatherService)
    {
        CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        _weatherService = weatherService;
        GetWeatherByLocation(); // 获取默认位置的天气
    }
    
    public string CurrentTime
    {
        get => _currentTime;
        set => SetProperty(ref _currentTime, value);
    }
    
    public string CurrentWeatherCondition
    {
        get => _currentWeatherCondition;
        set => SetProperty(ref _currentWeatherCondition, value);
    }
    
    public string CurrentLocation
    {
        get => _currentLocation;
        set
        {
            if (SetProperty(ref _currentLocation, value) && !string.IsNullOrWhiteSpace(CurrentLocation))
            {
                // 当地址发生变化时，更新天气
                GetWeatherByLocation();
            }
        }
    }

    // 获取当前位置的天气信息
    public async void GetWeatherByLocation()
    {
        // 假设根据地址获取经纬度，你可以扩展为调用地理位置 API
        // 以下只是示例
        var (latitude, longitude) = await _weatherService.GetCoordinatesFromLocation(CurrentLocation);

        // 调用天气服务获取天气信息
        var weatherInfo = await _weatherService.GetWeatherByLocationAsync(latitude, longitude);

        if (weatherInfo != null)
        {
            CurrentWeatherCondition = $"{weatherInfo.Condition}"; // 更新天气状况
        }
        else
        {
            CurrentWeatherCondition = "无法获取天气信息"; // 获取不到天气信息时的提示
        }
    }
    
}
