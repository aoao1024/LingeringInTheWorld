using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;
using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;

namespace LingeringInTheWorld.Library.ViewModels;

public class DiaryAddViewModel : ViewModelBase
{
    
    private readonly IAlertService _alertService;
    private readonly HeFengWeatherService _weatherService;
    // private readonly LocationService _locationService;
    
    private string _currentTime;
    private string _currentWeatherCondition;  // 天气状况
    
    public DiaryAddViewModel(IAlertService alertService)
    {
        _alertService = alertService;
        CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        _weatherService = new HeFengWeatherService(alertService);
        // _locationService = new LocationService(alertService);
        GetWeatherByLocation();
    }
    
    public string CurrentTime
    {
        get => _currentTime;
        set
        {
            _currentTime = value;
            OnPropertyChanged();
        }
    }
    
    public string CurrentWeatherCondition
    {
        get => _currentWeatherCondition;
        set => SetProperty(ref _currentWeatherCondition, value);
    }
    
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    public async void GetWeatherByLocation()
    {
        // 获取当前位置经纬度
        // var (latitude, longitude) = await _locationService.GetLocationAsync();
        var latitude = 42.67;
        var longitude = 123.46;

        var weatherInfo = await _weatherService.GetWeatherByLocationAsync(latitude, longitude);

        if (weatherInfo != null)
        {
            var condition = weatherInfo.Condition;
            CurrentWeatherCondition = $"{condition}";
        }
    }
}