using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.Input;
using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;

namespace LingeringInTheWorld.Library.ViewModels;

public class DiaryAddViewModel : ViewModelBase
{
    private readonly IAppStorage _appStorage;
    private readonly IAlertService _alertService;
    private readonly IWeatherService _weatherService;
    private readonly ILocationService _locationService;
    private readonly IContentNavigationService _contentNavigationService;

    private string _currentTime; // 当前时间
    private string _currentWeatherCondition; // 天气状况
    private string _title;
    private string _content;
    private string _currentLocation; // 当前地址
    private string _newLocation; // 新输入的地址
    private string _newTag; // 新标签
    private ObservableCollection<byte[]> _uploadedImages; // 用于存储上传的多张图片数据

    public ObservableCollection<string> Tags { get; set; }


    public DiaryAddViewModel(IAppStorage appStorage,IAlertService alertService, 
            IWeatherService weatherService, ILocationService locationService,
            IContentNavigationService contentNavigationService)
    {
        _appStorage = appStorage;
        _alertService = alertService;
        _weatherService = weatherService;
        _locationService = locationService;
        _contentNavigationService = contentNavigationService;
        
        Tags = new ObservableCollection<string>();
        UploadedImages = new ObservableCollection<byte[]>();
        UpdateLocationCommand = new RelayCommand(UpdateLocation);
        AddTagCommand = new RelayCommand(AddTag);
        RemoveTagCommand = new RelayCommand<string>(RemoveTag);
        UploadImageCommand = new AsyncRelayCommand<Control>(UploadImage);
        RemoveImageCommand = new RelayCommand<byte[]>(RemoveImage);
        SaveDiaryCommand = new RelayCommand(SaveDiaryAsync);
        Initialize();
    }

    // 初始化方法，确保每次加载时重新初始化
    private async void Initialize()
    {
        CurrentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // 更新当前时间
        Title = string.Empty; // 清空标题
        Content = string.Empty; // 清空正文
        Tags.Clear(); // 清空标签
        UploadedImages.Clear(); // 清空图片
        await GetLocationAndWeatherByIp();  // 获取位置和天气
    }

    // 图片数据属性（支持多张图片）
    public ObservableCollection<byte[]> UploadedImages
    {
        get => _uploadedImages;
        set => SetProperty(ref _uploadedImages, value);
    }
    
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
    
    public string Content
    {
        get => _content;
        set => SetProperty(ref _content, value);
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


    // 新的地址输入字段，绑定到 TextBox
    public string NewLocation
    {
        get => _newLocation;
        set => SetProperty(ref _newLocation, value);
    }
    
    public ICommand SaveDiaryCommand { get; }

    // 保存日记的方法
    private async void SaveDiaryAsync()
    {
        // 弹出确认框
        bool isConfirmed = await _alertService.ConfirmAsync("确认保存", "您确定要保存日记吗？");

        if (isConfirmed)
        {
            // 用户点击了“是”，执行保存操作
            var newDiary = new Diary
            {
                Title = Title,
                Content = Content,
                DateTime = DateTime.Now,
                Weather = CurrentWeatherCondition,
                Location = CurrentLocation,
                Tags = string.Join(",", Tags),
                ImagePaths = string.Join(",", UploadedImages.Select(image => Convert.ToBase64String(image)))
            };

            // 保存到数据库
            await _appStorage.InsertDiaryAsync(newDiary);

            // 提示用户保存成功
            await _alertService.AlertAsync("保存成功", "日记已成功保存！");
            
            _contentNavigationService.NavigateTo(ContentNavigationConstant.DiaryView);
            
        }
    }

    private void UpdateLocation()
    {
        // 将 NewLocation 更新到 CurrentLocation
        if (!string.IsNullOrWhiteSpace(NewLocation))
        {
            CurrentLocation = NewLocation;
        }
    }

    public ICommand UpdateLocationCommand { get; }

    private async Task GetLocationAndWeatherByIp()
    {
        var (latitude, longitude, locationName) = await _locationService.GetCurrentLocationAsync();
        CurrentLocation = locationName;
        NewLocation = locationName;
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

    // 从本地存储中选择并上传图片
    private async Task UploadImage(Control control)
    {
        // 获取父级窗口引用
        var window = control.GetVisualRoot() as Window;
        if (window == null)
        {
            return;
        }

        // 打开文件选择器
        var result = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            AllowMultiple = true,
            FileTypeFilter = new[]
                { new FilePickerFileType("Image Files") { Patterns = new[] { "*.jpg", "*.png", "*.jpeg" } } }
        });

        foreach (var file in result)
        {
            await using var fileStream = await file.OpenReadAsync();
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);

            // 将图片字节流添加到 UploadedImages 集合中
            UploadedImages.Add(memoryStream.ToArray());
        }
    }


    public ICommand UploadImageCommand { get; }

    // 删除图片
    private void RemoveImage(byte[] image)
    {
        UploadedImages.Remove(image); // 从集合中移除图片
    }

    public ICommand RemoveImageCommand { get; }

    // 获取当前位置的天气信息
    private async void GetWeatherByLocation()
    {
        // 根据ip地址获取经纬度，可以扩展为调用地理位置 API
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

    // 添加标签命令
    public ICommand AddTagCommand { get; }

    // 添加标签逻辑
    private void AddTag()
    {
        if (string.IsNullOrWhiteSpace(NewTag) || Tags.Count >= 10) return;
        Tags.Add(NewTag);
        NewTag = string.Empty; // 清空输入框
    }

    // 删除标签命令
    public ICommand RemoveTagCommand { get; }


    // 删除标签逻辑
    private void RemoveTag(string tag)
    {
        Tags.Remove(tag);
    }
}