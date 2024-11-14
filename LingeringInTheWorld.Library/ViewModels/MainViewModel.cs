using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;

namespace LingeringInTheWorld.Library.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly ITodayImageService _todayImageService;

    public MainViewModel(ITodayImageService todayImageService)
    {
        _todayImageService = todayImageService;
        
        OnInitializedCommand = new RelayCommand(OnInitialized);
    }
    
    private TodayImage _todayImage;

    public TodayImage TodayImage {
        get => _todayImage;
        private set => SetProperty(ref _todayImage, value);
    }
    
    //加载数据进度条
    private bool _isLoading;

    public bool IsLoading {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }
    
    public ICommand OnInitializedCommand { get; }
    
    public void OnInitialized()
    {
       
        Task.Run(async () =>
        {
            TodayImage = await _todayImageService.GetTodayImageAsync(); //获取今日图片
            var updateResult = await _todayImageService.CheckUpdateAsync();
            if (updateResult.HasUpdate) {
                TodayImage = updateResult.TodayImage;
            }
        });
        
    }

}