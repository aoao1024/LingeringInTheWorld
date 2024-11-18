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

    private Expression<Func<Diary, bool>> _where;
    
    public override void SetParameter(object parameter) {
        if (parameter is not Expression<Func<Diary, bool>> where) {
            return;
        }

        _where = where;
        _canLoadMore = true;
        DiaryCollection.Clear();
    }
    
    public DiaryViewModel(IAppStorage appStorage, IContentNavigationService contentNavigationService)
    {
        _appStorage = appStorage;
        _contentNavigationService = contentNavigationService;
        _canLoadMore = true;
        Console.WriteLine("_canLoadMore--------"+_canLoadMore);
        DiaryCollection = new AvaloniaInfiniteScrollCollection<Diary>
        {
            OnCanLoadMore = () => true,
            OnLoadMore = async () =>
            {
                Console.WriteLine("OnLoadMore--------");
                Status = Loading;
                var diaries = await appStorage.GetDiariesAsync(
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
        
        AddDiaryCommand = new RelayCommand(AddDiary);
        ShowDiaryDetailCommand = new RelayCommand<Diary>(ShowDiaryDetail);
    }
    
    private bool _canLoadMore;
    
    private string _status;
    
    private const int PageSize = 20;
    
    public string Status {
        get => _status;
        private set => SetProperty(ref _status, value);
    }

    private const string Loading = "正在载入";

    private const string NoResult = "没有满足条件的结果";

    private const string NoMoreResult = "没有更多结果";

    
    public AvaloniaInfiniteScrollCollection<Diary> DiaryCollection { get; }
    
    public IRelayCommand<Diary> ShowDiaryDetailCommand { get; }

    private void ShowDiaryDetail(Diary diary) =>
        _contentNavigationService.NavigateTo(ContentNavigationConstant.DiaryDetailView, diary);

    public IRelayCommand AddDiaryCommand { get; }

    private void AddDiary()
    {
        _contentNavigationService.NavigateTo(ContentNavigationConstant.DiaryAddView);
    }
}