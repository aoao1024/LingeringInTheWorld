using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;

namespace LingeringInTheWorld.Library.ViewModels;

public class DiaryAddViewModel : ViewModelBase
{
    private readonly IAppStorage _appStorage;
    private readonly IAlertService _alertService;
    private readonly IWeatherService _weatherService;
    private readonly ILocationService _locationService;
    private readonly MenuViewModel _menuViewModel;
    private readonly IAIAnalysisService _aiAnalysisService;
    private readonly IToDoStorage _todoStorage;
    private readonly IAccountingStorage _accountingStorage;

    private string _currentTime; // 当前时间
    private string _currentWeatherCondition; // 天气状况
    private string _title; // 标题
    private string _content; // 正文
    private string _currentLocation; // 当前地址
    private string _newLocation; // 新输入的地址
    private string _newTag; // 新标签
    private ObservableCollection<byte[]> _uploadedImages; // 用于存储上传的多张图片数据
    private ObservableCollection<string> _tags; // 标签集合
    
    // 用于存储账单和代办事项
    private ObservableCollection<Accounting> _accountings;
    private ObservableCollection<ToDo> _toDos;

    public override void SetParameter(object parameter) {
        Diary = null;
        if (parameter is not Models.Diary diary) {
            return;
        }
        Diary = diary;
    }
    
    private Diary _diary;

    private Diary Diary {
        get => _diary;
        set => SetProperty(ref _diary, value);
    }
    
    public DiaryAddViewModel(IAppStorage appStorage,IAlertService alertService, 
            IWeatherService weatherService, ILocationService locationService,
            MenuViewModel menuViewModel, IAIAnalysisService aiAnalysisService,
            IToDoStorage todoStorage, IAccountingStorage accountingStorage)
    {
        _appStorage = appStorage;
        _alertService = alertService;
        _weatherService = weatherService;
        _locationService = locationService;
        _menuViewModel = menuViewModel;
        _aiAnalysisService = aiAnalysisService;
        _todoStorage = todoStorage;
        _accountingStorage = accountingStorage;
        
        Tags = new ObservableCollection<string>();
        UploadedImages = new ObservableCollection<byte[]>();
        Accountings = new ObservableCollection<Accounting>();
        ToDos = new ObservableCollection<ToDo>();
        
        OnInitializeCommand = new RelayCommand(OnInitialize);
        UpdateLocationCommand = new RelayCommand(UpdateLocation);
        AddTagCommand = new RelayCommand(AddTag);
        RemoveTagCommand = new RelayCommand<string>(RemoveTag);
        UploadImageCommand = new AsyncRelayCommand<Control>(UploadImage);
        RemoveImageCommand = new RelayCommand<byte[]>(RemoveImage);
        SaveDiaryCommand = new RelayCommand(SaveDiaryAsync);
        RemoveAccountingCommand = new RelayCommand<Accounting>(RemoveAccounting);
        RemoveToDoCommand = new RelayCommand<ToDo>(RemoveToDo);
        SaveAccountingCommand = new RelayCommand<Accounting>(SaveAccounting);
        SaveToDoCommand = new RelayCommand<ToDo>(SaveToDo);
        
        AIAnalyzeCommand = new RelayCommand(OnAIAnalyze);
    }

    public ICommand OnInitializeCommand { get; }
    
    // 初始化方法，确保每次加载时重新初始化
    private async void OnInitialize()
    {
        // 初始化时清空所有属性
        CurrentTime = string.Empty; // 清空时间
        Title = string.Empty; // 清空标题
        Content = string.Empty; // 清空正文
        Tags.Clear(); // 清空标签
        UploadedImages.Clear(); // 清空图片
        Accountings.Clear(); // 清空账单
        ToDos.Clear(); // 清空代办事项
        
        if (Diary != null)
        {
            // 如果是编辑日记，则将日记的内容填充到对应的属性中
            CurrentTime = Diary.DateTime.ToString("ddd yyyy-MM-dd HH:mm:ss");
            CurrentWeatherCondition = Diary.Weather;
            Title = Diary.Title;
            Content = Diary.Content;
            Tags = new ObservableCollection<string>(Diary.TagsList());
            NewLocation = Diary.Location;
            UploadedImages = new ObservableCollection<byte[]>(Diary.ImageBytesList());
        }
        else
        {
            // 如果是新建日记，则初始化时间、位置、天气属性
            CurrentTime = DateTime.Now.ToString("ddd yyyy-MM-dd HH:mm:ss"); // 更新当前时间
            await GetLocationAndWeatherByIp();  // 获取位置和天气
        }
    }
    
    //加载数据进度条
    private bool _isLoading;

    public bool IsLoading {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }
    
    // AI分析命令
    public ICommand AIAnalyzeCommand { get; }
    private async void OnAIAnalyze()
    {
        if (string.IsNullOrEmpty(Content)) 
        {
            await _alertService.AlertAsync("提示", "请先填写日记内容！");
            return;
        }

        IsLoading = true;
        var response = await _aiAnalysisService.AnalyzeDiaryAsync(CurrentTime, Content);

        if (response != null)
        {
            // 如果账单和待办事项不为空则清空
            Accountings?.Clear();
            ToDos?.Clear();   
            
            // 解析返回的数据，填充账单和代办事项
            foreach (var accounting in response.Accountings)
            {
                Accountings!.Add(accounting);
            }

            foreach (var toDo in response.ToDos)
            {
                ToDos!.Add(toDo);
            }
            
        }
        else
        {
            await _alertService.AlertAsync("错误", "AI分析失败，请稍后再试。");
        }
        IsLoading = false;
    }

    // 图片数据属性（支持多张图片）
    public ObservableCollection<byte[]> UploadedImages
    {
        get => _uploadedImages;
        set => SetProperty(ref _uploadedImages, value);
    }
    
    // 标签集合属性
    public ObservableCollection<string> Tags
    {
        get => _tags;
        set => SetProperty(ref _tags, value);
    }
    
    public ObservableCollection<Accounting> Accountings
    {
        get => _accountings;
        set => SetProperty(ref _accountings, value);
    }
        
    public ObservableCollection<ToDo> ToDos
    {
        get => _toDos;
        set => SetProperty(ref _toDos, value);
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
                // 如果是编辑日记，则不更新天气
                if(Diary == null)
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
        var isConfirmed = await _alertService.ConfirmAsync("确认保存", "您确定要保存日记吗？");
        if (!isConfirmed) return;
        // 如果是编辑日记
        if (Diary != null)
        {
            // 更新日记的属性
            Diary.Title = Title;
            Diary.Content = Content;
            Diary.DateTime = DateTime.Now;
            Diary.Weather = CurrentWeatherCondition;
            Diary.Location = CurrentLocation;
            Diary.Tags = string.Join(" | ", Tags);
            Diary.Images = string.Join(",", UploadedImages.Select(Convert.ToBase64String));

            // 更新到数据库
            await _appStorage.UpdateDiaryAsync(Diary);
                
            if (await _appStorage.QueryDiaryByIdAsync(Diary.Id) != null)
            {
                // 提示用户保存成功
                await _alertService.AlertAsync("保存成功", "日记已成功保存！");
                _menuViewModel.GoBack();
            }
            else
            {
                // 提示用户保存失败
                await _alertService.AlertAsync("保存失败", "日记保存失败！");
            }

        }
        else
        {
            // 如果是新建日记
            var newDiary = new Diary
            {
                Title = Title,
                Content = Content,
                DateTime = DateTime.Now,
                Weather = CurrentWeatherCondition,
                Location = CurrentLocation,
                Tags = string.Join(" | ", Tags),
                Images = string.Join(",", UploadedImages.Select(Convert.ToBase64String))
            };

            // 保存到数据库
            await _appStorage.InsertDiaryAsync(newDiary);

            if (await _appStorage.QueryDiaryByIdAsync(newDiary.Id) != null)
            {
                // 提示用户保存成功
                await _alertService.AlertAsync("保存成功", "日记已成功保存！");
                _menuViewModel.GoBack();
            }
            else
            {
                // 提示用户保存失败
                await _alertService.AlertAsync("保存失败", "日记保存失败！");
            }
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

        CurrentWeatherCondition = weatherInfo != null ? $"{weatherInfo.Condition}" : // 更新天气状况
            "无法获取天气信息"; // 获取不到天气信息时的提示
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
    
    // 删除账单命令
    public ICommand RemoveAccountingCommand { get; }
    // 删除账单逻辑
    private async void RemoveAccounting(Accounting accounting)
    {
        var isConfirmed = await _alertService.ConfirmAsync("确认删除", "您确定要删除该账单吗？");
        if (!isConfirmed) return;
        Accountings.Remove(accounting);
    }
    
    //保存账单命令
    public ICommand SaveAccountingCommand { get; }
    // 保存账单逻辑
    private async void SaveAccounting(Accounting accounting)
    {
        var isConfirmed = await _alertService.ConfirmAsync("确认保存", "您确定要保存该账单吗？");
        if (!isConfirmed) return;
        await _accountingStorage.SaveAccountingAsync(accounting);
        if (await _accountingStorage.GetAccounting(accounting.Id) != null)
        {
            // 提示用户保存成功
            await _alertService.AlertAsync("保存成功", "账单已成功保存！");
            Accountings.Remove(accounting);
        }
        else
        {
            // 提示用户保存失败
            await _alertService.AlertAsync("保存失败", "账单保存失败！");
        }
    }
    
    // 删除待办事项命令
    public ICommand RemoveToDoCommand { get; }
    // 删除待办事项逻辑
    private async void RemoveToDo(ToDo toDo)
    {
        var isConfirmed = await _alertService.ConfirmAsync("确认删除", "您确定要删除该代办吗？");
        if (!isConfirmed) return;
        ToDos.Remove(toDo);
    }
    
    // 保存待办事项命令
    public ICommand SaveToDoCommand { get; }
    // 保存待办事项逻辑
    private async void SaveToDo(ToDo toDo)
    {
        var isConfirmed = await _alertService.ConfirmAsync("确认保存", "您确定要保存该代办吗？");
        if (!isConfirmed) return;
        await _todoStorage.AddToDoItemAsync(toDo);
        if (await _todoStorage.GetToDoItemAsync(toDo.Id) != null)
        {
            // 提示用户保存成功
            await _alertService.AlertAsync("保存成功", "待办事项已成功保存！");
            ToDos.Remove(toDo);
        }
        else
        {
            // 提示用户保存失败
            await _alertService.AlertAsync("保存失败", "待办事项保存失败！");
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
        // 根据地址获取经纬度
        var (latitude, longitude) = await _weatherService.GetCoordinatesFromLocation(CurrentLocation);

        // 调用天气服务获取天气信息
        var weatherInfo = await _weatherService.GetWeatherByLocationAsync(latitude, longitude);

        CurrentWeatherCondition = weatherInfo != null ? $"{weatherInfo.Condition}" : // 更新天气状况
            "无法获取天气信息"; // 获取不到天气信息时的提示
    }

   
}