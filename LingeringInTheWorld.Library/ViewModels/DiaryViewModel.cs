using System.Linq.Expressions;
using AvaloniaInfiniteScrolling;
using CommunityToolkit.Mvvm.Input;
using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;

namespace LingeringInTheWorld.Library.ViewModels;

public class DiaryViewModel : ViewModelBase
{
    private readonly IAppStorage _appStorage;
    private readonly IContentNavigationService _contentNavigationService;

    public DiaryViewModel(IAppStorage appStorage, IContentNavigationService contentNavigationService)
    {
        _contentNavigationService = contentNavigationService;
        _appStorage = appStorage;
        
        AddDiaryCommand = new RelayCommand(AddDiary);

        DiaryCollection = new AvaloniaInfiniteScrollCollection<Diary>
        {
            OnCanLoadMore = () => _canLoadMore,
            OnLoadMore = async () =>
            {
                Status = Loading;
                var diaries = await appStorage.ListDiaryAsync();
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
        ShowDiaryCommand = new RelayCommand<Diary>(ShowDiary);
    }
    
    private bool _canLoadMore = true;
    
    private string _status;
    
    public string Status {
        get => _status;
        private set => SetProperty(ref _status, value);
    }

    public const string Loading = "正在载入";

    public const string NoResult = "没有满足条件的结果";

    public const string NoMoreResult = "没有更多结果";

    public const int PageSize = 20;
    public AvaloniaInfiniteScrollCollection<Diary> DiaryCollection { get; }
    
    public IRelayCommand<Diary> ShowDiaryCommand { get; }
    
    public void ShowDiary(Diary diary) =>
        _contentNavigationService.NavigateTo(ContentNavigationConstant.DiaryDetailView, diary);

    public IRelayCommand AddDiaryCommand { get; }
    
    public void AddDiary()
    {
        _contentNavigationService.NavigateTo(ContentNavigationConstant.DiaryAddView);
    }
}