using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using LingeringInTheWorld.Library.Services;
using ReactiveUI;

namespace LingeringInTheWorld.Library.ViewModels;

public class DiaryAddViewModel : ViewModelBase
{
    private readonly IWeatherService _weatherService;
    private readonly ILocationService _locationService;
    
    private string _currentTime;    // 当前时间
    private string _currentWeatherCondition;    // 天气状况
    private string _currentLocation;    // 当前地址
    private string _newTag;    // 新标签
    public ObservableCollection<string> Tags { get; set; }


    public DiaryAddViewModel(IWeatherService weatherService, ILocationService locationService)
    {
        CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        _weatherService = weatherService;
        _locationService = locationService;
        GetLocationAndWeatherByIp(); 
        Tags = new ObservableCollection<string>();
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
    
    public string NewTag
    {
        get => _newTag;
        set => SetProperty(ref _newTag, value);
    }
    
    public async void GetLocationAndWeatherByIp()
    {
        var (latitude, longitude, locationName) = await _locationService.GetCurrentLocationAsync();
        CurrentLocation = locationName;
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
    
    // 假设你有一个异步操作
    public ReactiveCommand<Unit, Unit> AddTagCommand => ReactiveCommand.Create(AddTag);

    private void AddTag()
    {
        // 如果 AddTag 需要在 UI 线程中执行，使用 ObserveOn 来确保操作在 UI 线程
        Observable.Return(Unit.Default)
            .ObserveOn(RxApp.MainThreadScheduler)  // 这里切换到主线程
            .Subscribe(_ =>
            {
                if (!string.IsNullOrWhiteSpace(NewTag) && Tags.Count < 3)
                {
                    Tags.Add(NewTag);
                    NewTag = string.Empty; // 清空输入框
                }
            });
    }
    
    // 删除标签命令
    public ReactiveCommand<string, Unit> RemoveTagCommand => ReactiveCommand.Create<string>(RemoveTag);
    

    // 删除标签逻辑
    private void RemoveTag(string tag)
    {
        Tags.Remove(tag);
    }
}
