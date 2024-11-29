using System.Linq.Expressions;
using AvaloniaInfiniteScrolling;
using CommunityToolkit.Mvvm.Input;
using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;
using LinqKit;
using Microsoft.VisualBasic;

namespace LingeringInTheWorld.Library.ViewModels;

public class DiaryViewModel : ViewModelBase
{
    private readonly IAppStorage _appStorage;
    private readonly IAlertService _alertService;
    private readonly IContentNavigationService _contentNavigationService;

    public DateTime? DateSearch { get; set; }
    public string TitleSearchText { get; set; }
    public string TagSearchText { get; set; }
    public string LocationSearchText { get; set; }

    
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
        EditDiaryCommand = new RelayCommand<Diary>(EditDiary);
        DeleteDiaryCommand = new RelayCommand<Diary>(DeleteDiary);
        SearchDiariesCommand = new RelayCommand(SearchDiaries);
        ResetSearchCommand = new RelayCommand(ResetSearchFilters);
        DiaryCollection = new AvaloniaInfiniteScrollCollection<Diary>
        {
            OnCanLoadMore = () => _canLoadMore,
            OnLoadMore = async () =>
            {
                Console.WriteLine("OnLoadMore--------");
                Status = Loading;
                var diaries = await _appStorage.GetDiariesAsync(
                    _where ?? (d => true), // 使用传入的查询条件，默认加载所有数据
                    DiaryCollection.Count,  // 从当前已加载的数量开始
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
    
    public IRelayCommand SearchDiariesCommand { get; }
    private void SearchDiaries()
    {
        // 调用 SetSearchFilters 方法设置过滤条件并加载数据
        SetSearchFilters(DateSearch, TitleSearchText, TagSearchText, LocationSearchText);
    }
    
    public IRelayCommand ResetSearchCommand { get; }
    private void ResetSearchFilters()
    {
        // 清空所有搜索条件
        DateSearch = null;
        TitleSearchText = string.Empty;
        TagSearchText = string.Empty;
        LocationSearchText = string.Empty;

        // 触发属性更改通知，更新界面
        OnPropertyChanged(nameof(DateSearch));
        OnPropertyChanged(nameof(TitleSearchText));
        OnPropertyChanged(nameof(TagSearchText));
        OnPropertyChanged(nameof(LocationSearchText));

        // 调用查询方法，重新加载全部数据
        SetSearchFilters(null, string.Empty, string.Empty, string.Empty);
    }
    
    public void SetSearchFilters(DateTime? dateSearch, string title, string tag, string location)
    {
        // 清空现有的日记集合
        DiaryCollection.Clear();
        // 根据用户输入构建查询条件
        var predicate = PredicateBuilder.New<Diary>(d => true);
        if (dateSearch.HasValue)
        {
            predicate = predicate.And(d => d.DateTime >= dateSearch.Value);
        }
        if (!string.IsNullOrEmpty(title))
        {
            predicate = predicate.And(d => d.Title.Contains(title));
        }
        if (!string.IsNullOrEmpty(tag))
        {
            predicate = predicate.And(d => d.Tags.Contains(tag));
        }
        if (!string.IsNullOrEmpty(location))
        {
            predicate = predicate.And(d => d.Location.Contains(location));
        }

        _where = predicate; // 更新查询条件
        _canLoadMore = true;      // 允许加载更多
        DiaryCollection.LoadMoreAsync(); // 加载数据
    }
    
    //事件处理函数，void
    private void AppStorageOnUpdated(object sender, Diary diary)
    {
        var index = 0;
        if (DiaryCollection.FirstOrDefault(d => d.Id == diary.Id) != null)
        {
            index = DiaryCollection.IndexOf(DiaryCollection.FirstOrDefault(d => d.Id == diary.Id));
        }
        DiaryCollection.Remove(DiaryCollection.FirstOrDefault(d => d.Id == diary.Id));
        var d = _appStorage.QueryDiaryByIdAsync(diary.Id);
        if (d == null)
        {
            return;
        }
        DiaryCollection.Insert(index, diary);
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
    
    public IRelayCommand<Diary> EditDiaryCommand { get; }
    private void EditDiary(Diary diary)
    {
        _contentNavigationService.NavigateTo(ContentNavigationConstant.DiaryAddView, diary);
    }
    
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