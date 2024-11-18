using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Windows.Input;
using AvaloniaInfiniteScrolling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;

namespace LingeringInTheWorld.Library.ViewModels;

public class ToDoListViewModel : ViewModelBase
{
    /*public ObservableCollection<ToDoItemViewModel> ToDoCollection
    { get; }*/
    private AvaloniaInfiniteScrollCollection<ToDoItemViewModel> _toDoCollection;

    public AvaloniaInfiniteScrollCollection<ToDoItemViewModel> ToDoCollection
    {
        get => _toDoCollection;
        set => SetProperty(ref _toDoCollection, value);
    }

    private ITodoStorageService _todoStorageService;

    private string _status;
    private bool _canLoadMore = true;
    public const int PageSize = 10;

    public string Status
    {
        get => _status;
        private set => SetProperty(ref _status, value);
    }

    public const string Loading = "正在载入";

    public const string NoResult = "没有满足条件的结果";

    public const string NoMoreResult = "没有更多结果";

    public ToDoListViewModel(ITodoStorageService todoStorageService, IContentNavigationService contentNavigationService)
    {
        _todoStorageService = todoStorageService;
        _contentNavigationService = contentNavigationService;
        OnInitializeCommand = new AsyncRelayCommand(OnInitializeAsync);
        AddToDoCommand = new RelayCommand(AddToDo);
        ToDoCollection = new AvaloniaInfiniteScrollCollection<ToDoItemViewModel>
        {
            OnCanLoadMore = () => _canLoadMore,
            OnLoadMore = async () =>
            {
                var AllToDoItems = await _todoStorageService.GetToDoList(Expression.Lambda<Func<ToDo, bool>>(
                        Expression.Constant(true),
                        Expression.Parameter(typeof(ToDo), "todo")),
                    ToDoCollection.Count, PageSize);
                if (AllToDoItems.Count < PageSize)
                {
                    _canLoadMore = false;
                }

                IList<ToDoItemViewModel> toDoItemViewModels = new List<ToDoItemViewModel>();
                foreach (var toDoItem in AllToDoItems)
                {
                    ToDoItemViewModel toDoItemViewModel = new ToDoItemViewModel(toDoItem, this);
                    toDoItemViewModels.Add(toDoItemViewModel);
                }

                return toDoItemViewModels;
            }
        };

    }

    public ICommand OnInitializeCommand { get; }

    public async Task OnInitializeAsync()
    {
        /*ToDoCollection = new AvaloniaInfiniteScrollCollection<ToDoItemViewModel>
        {
            OnCanLoadMore = () => _canLoadMore,
            OnLoadMore = async () =>
            {
                Status = Loading;
                var AllToDoItems = await _todoStorageService.GetToDoList(Expression.Lambda<Func<ToDo, bool>>(
                        Expression.Constant(true),
                        Expression.Parameter(typeof(ToDo), "todo")),
                    ToDoCollection.Count, PageSize);
                Status = string.Empty;
                if (AllToDoItems.Count < PageSize)
                {
                    _canLoadMore = false;
                    Status = NoMoreResult;
                }
                if (ToDoCollection.Count == 0 && AllToDoItems.Count == 0)
                {
                    Status = NoResult;
                }
                IList<ToDoItemViewModel> toDoItemViewModels = new List<ToDoItemViewModel> ();
                foreach (var toDoItem in AllToDoItems)
                {
                    ToDoItemViewModel toDoItemViewModel = new ToDoItemViewModel(toDoItem,this);
                    toDoItemViewModels.Add(toDoItemViewModel);
                }
                Console.WriteLine($"OnLoadMore: Loaded {toDoItemViewModels.Count} items");
                Console.WriteLine(toDoItemViewModels.Count);
                return toDoItemViewModels;
            }
        };*/
        await ToDoCollection.LoadMoreAsync();
        //Console.WriteLine(ToDoCollection.Count);
        /*if (ToDoCollection.Count!=0)
        {
            ToDoCollection.Clear();
        }
       var AllToDoItems = await _todoStorageService.GetAllTodoListAsync();
        foreach (var toDoItem in AllToDoItems)
        {
            ToDoItemViewModel toDoItemViewModel = new ToDoItemViewModel(toDoItem,this);
            if (toDoItemViewModel != null) ToDoCollection.Add(toDoItemViewModel);
        }*/

    }

    //1115
    private readonly IContentNavigationService _contentNavigationService;
    public ICommand AddToDoCommand { get; }

    public void AddToDo()
    {
        _contentNavigationService.NavigateTo(ContentNavigationConstant.NewToDoItemView);
    }

    public async Task SetToDoItemFinishStatusAsync(ToDoItemViewModel toDoItemViewModel)
    {
        Console.WriteLine("更改状态为已完成");
        await _todoStorageService.UpdateToDoItemStatusAsync(toDoItemViewModel.ToDo.Id, !toDoItemViewModel.ToDo.Status);
        ToDoCollection.RemoveAt(ToDoCollection.IndexOf(toDoItemViewModel));
        toDoItemViewModel.ToDo.Status = !toDoItemViewModel.ToDo.Status;
        ToDoCollection.Add(toDoItemViewModel);
    }

    public async Task<int> DeleteToDoItemAsync(ToDoItemViewModel toDoItemViewModel)
    {
        int deleteIndex = await _todoStorageService.DeleteToDoItemAsync(toDoItemViewModel.ToDo.Id);
        ToDoCollection.RemoveAt(ToDoCollection.IndexOf(toDoItemViewModel));
        return deleteIndex;
    }

    public void ShowToDoItemDetailInfo(ToDoItemViewModel toDo)
    {
        _contentNavigationService.NavigateTo(ContentNavigationConstant.ToDoDetailView,toDo.ToDo);
    }
}

//1118
//     private readonly IToDoStorage _toDoStorage;
//
//     private readonly IContentNavigationService _contentNavigationService;
//
//     
//
//     public ToDoListViewModel(IToDoStorage toDoStorage, IContentNavigationService contentNavigationService) {
//         _contentNavigationService = contentNavigationService;
//         ToDoCollection = new AvaloniaInfiniteScrollCollection<ToDo> {
//             OnCanLoadMore = () => _canLoadMore,
//             OnLoadMore = async () => {
//                 Status = Loading;
//                 var toDos = await toDoStorage.GetTodoListAsync(null,
//                     ToDoCollection.Count, PageSize);
//                 Status = string.Empty;
//
//                 if (toDos.Count < PageSize) {
//                     _canLoadMore = false;
//                     Status = NoMoreResult;
//                 }
//
//                 if (ToDoCollection.Count == 0 && toDos.Count == 0) {
//                     Status = NoResult;
//                 }
//
//                 return toDos;
//
//             }
//         };
//         ShowToDoCommand = new RelayCommand<ToDo>(ShowToDo);
//     }
//
//     private bool _canLoadMore;
//
//     
//
//     public AvaloniaInfiniteScrollCollection<ToDo> ToDoCollection { get; }
//
//     private string _status;
//
//     public string Status {
//         get => _status;
//         private set => SetProperty(ref _status, value);
//     }
//
//     public const string Loading = "正在载入";
//
//     public const string NoResult = "没有满足条件的结果";
//
//     public const string NoMoreResult = "没有更多结果";
//
//     public const int PageSize = 20;
//
//     public IRelayCommand<ToDo> ShowToDoCommand { get; }
//
//     public void ShowToDo(ToDo toDo) =>
//         _contentNavigationService.NavigateTo(ContentNavigationConstant.ToDoDetailView, toDo);
// }

public class ToDoItemViewModel :ObservableObject
{
    private readonly ToDoListViewModel _toDoListViewModel;
    private ToDo _todo;
    public ToDo ToDo
    {
        get => _todo;
        set => SetProperty(ref _todo, value);
    }
    public ToDoItemViewModel(ToDo todo,ToDoListViewModel toDoListViewModel)
    {
        ToDo = todo;
        _toDoListViewModel = toDoListViewModel;
        UpdateToDoItemStatusCommand = new AsyncRelayCommand(UpdateToDoItemStatusAsync);
        DeletToDoItemCommand = new AsyncRelayCommand(DeletToDoItemAsync);
        ShowToDoItemDetailCommand = new RelayCommand<ToDoItemViewModel>(ShowToDoItemDetailInfo);
    }
    public ICommand UpdateToDoItemStatusCommand { get; }
    public async Task UpdateToDoItemStatusAsync()
    {
        await _toDoListViewModel.SetToDoItemFinishStatusAsync(this);
    }
    public ICommand DeletToDoItemCommand { get; }
    public async Task<int> DeletToDoItemAsync()
        => await _toDoListViewModel.DeleteToDoItemAsync(this);

    public IRelayCommand<ToDoItemViewModel> ShowToDoItemDetailCommand { get; }
    public void ShowToDoItemDetailInfo(ToDoItemViewModel todo)
    {
        _toDoListViewModel.ShowToDoItemDetailInfo(todo);
    }
    

}