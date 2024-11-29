using System.Linq.Expressions;
using AvaloniaInfiniteScrolling;
using CommunityToolkit.Mvvm.Input;
using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;

namespace LingeringInTheWorld.Library.ViewModels;

public class DiaryViewModel : ViewModelBase
{
    private readonly IAppStorage _appStorage;
    private readonly IAlertService _alertService;
    private readonly IContentNavigationService _contentNavigationService;

    private Expression<Func<Diary, bool>> _where;
   
    public DiaryViewModel(IAppStorage appStorage, IAlertService alertService, 
        IContentNavigationService contentNavigationService)
    {
        _appStorage = appStorage;
        _alertService = alertService;
        _contentNavigationService = contentNavigationService;
        // OnInitializeCommand=new AsyncRelayCommand(OnInitializeAsync);
        _appStorage.Updated += AppStorageOnUpdated;
        AddDiaryCommand = new RelayCommand(AddDiary);
        ShowDiaryDetailCommand = new RelayCommand<Diary>(ShowDiaryDetail);
        DeleteDiaryCommand = new RelayCommand<Diary>(DeleteDiary);
        DiaryCollection = new AvaloniaInfiniteScrollCollection<Diary>
        {
            OnCanLoadMore = () => _canLoadMore,
            OnLoadMore = async () =>
            {
                Console.WriteLine("OnLoadMore--------");
                Status = Loading;
                var diaries = await _appStorage.GetDiariesAsync(
                    Expression.Lambda<Func<Diary, bool>>(
                        Expression.Constant(true),  // 不带条件，加载所有日记
                        Expression.Parameter(typeof(Diary), "d")
                    ),
                    DiaryCollection.Count,  // 从当前已经加载的数据数开始
                    PageSize                // 每次加载的数据量
                );
                Status = string.Empty;

                if (diaries.Count < PageSize)
                {
                    _canLoadMore = false;
                    Status = NoMoreResult;
                }

                if (DiaryCollection.Count == 0 && diaries.Count == 0)
                {
                    Status = NoResult;
                }

                return diaries;
            }
            
        };
    }
    
    //事件处理函数，void
    private async void AppStorageOnUpdated(object sender, Diary diary)
    {
        // 从Collection中删除这个日记
        DiaryCollection.Remove(DiaryCollection.FirstOrDefault(d => d.Id == diary.Id));
       //  查看数据库中有没有这个日记
        var d = _appStorage.QueryDiaryByIdAsync(diary.Id);
        //  如果没有，直接返回
        if (d == null)
        {
            return;
        }
        //  如果有，插入到Collection中
        DiaryCollection.Insert(0, diary);
    }
    
    private bool _canLoadMore = true;
    private string _status;
    private const int PageSize = 20;
    public string Status {
        get => _status;
        private set => SetProperty(ref _status, value);
    }

    private const string Loading = "正在载入...";
    private const string NoResult = "没有满足条件的结果";
    private const string NoMoreResult = "没有更多结果";

    
    public AvaloniaInfiniteScrollCollection<Diary> DiaryCollection { get; }
    
    // public ICommand OnInitializeCommand { get; }
    // public async Task OnInitializeAsync()
    // {
    //     await DiaryCollection.LoadMoreAsync();
    // }

    public IRelayCommand<Diary> ShowDiaryDetailCommand { get; }
    private void ShowDiaryDetail(Diary diary) =>
        _contentNavigationService.NavigateTo(ContentNavigationConstant.DiaryDetailView, diary);
    
    
    public IRelayCommand<Diary> DeleteDiaryCommand { get; }
    // 删除日记的实现
    private async void DeleteDiary(Diary diary)
    {
        // 弹出确认框
        bool isConfirmed = await _alertService.ConfirmAsync("确认删除", "您确定要删除日记吗？");

        if (isConfirmed)
        {
            // 用户点击了“是”，执行删除操作
            await _appStorage.DeleteDiaryAsync(diary.Id);
            if (await _appStorage.QueryDiaryByIdAsync(diary.Id) == null)
            {
                // 提示用户删除成功
                await _alertService.AlertAsync("删除成功", "日记已成功删除！");
                DiaryCollection.Remove(diary);
            }
            else
            {
                // 提示用户删除失败
                await _alertService.AlertAsync("删除失败", "日记删除失败！");
            }
            
        }
    }
    
    public IRelayCommand AddDiaryCommand { get; }
    private void AddDiary()
    {
        _contentNavigationService.NavigateTo(ContentNavigationConstant.DiaryAddView);
    }
    
}