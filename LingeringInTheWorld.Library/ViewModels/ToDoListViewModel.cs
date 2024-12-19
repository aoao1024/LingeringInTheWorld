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
        await ToDoCollection.LoadMoreAsync();
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
        await _todoStorageService.UpdateToDoItemStatusAsync(toDoItemViewModel.ToDo.Id, !toDoItemViewModel.ToDo.Status);
        await _todoStorageService.UpdateToDoItemFinishedTimeAsync(toDoItemViewModel.ToDo.Id, DateTime.Now);
        int index = ToDoCollection.IndexOf(toDoItemViewModel);
        ToDoCollection.RemoveAt(index);
        ToDo toDo = await _todoStorageService.GetToDoItem(toDoItemViewModel.ToDo.Id);
        toDoItemViewModel.ToDo = toDo;
        ToDoCollection.Insert(index,toDoItemViewModel);
    }

    public async Task<int> DeleteToDoItemAsync(ToDoItemViewModel toDoItemViewModel)
    {
        int deleteResult = await _todoStorageService.DeleteToDoItemAsync(toDoItemViewModel.ToDo.Id);
        if (1==deleteResult)
        {
            ToDoCollection.RemoveAt(ToDoCollection.IndexOf(toDoItemViewModel));
        }
        return deleteResult;
    }

    public void ShowToDoItemDetailInfo(ToDoItemViewModel toDo)
    {
        _contentNavigationService.NavigateTo(ContentNavigationConstant.ToDoDetailView,toDo.ToDo);
    }
}
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