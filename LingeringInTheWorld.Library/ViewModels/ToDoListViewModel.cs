using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
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
   public AvaloniaInfiniteScrollCollection<ToDoItemViewModel> ToDoCollection { get; }
    private ITodoStorageService _todoStorageService;
    private IList<ToDo> AllToDoItems;
    private string _status;
    private bool _canLoadMore=true;

    public string Status
    {
        get => _status;
        private set => SetProperty(ref _status, value);
    }
    public const string Loading = "正在载入";

    public const string NoResult = "没有满足条件的结果";

    public const string NoMoreResult = "没有更多结果";
    public  ToDoListViewModel(ITodoStorageService todoStorageService,IContentNavigationService contentNavigationService)
    {
        _todoStorageService = todoStorageService;
        _contentNavigationService = contentNavigationService;
        OnInitializeCommand=new AsyncRelayCommand(OnInitializeAsync);
        AddToDoCommand=new RelayCommand(AddToDo);
    }
    public ICommand OnInitializeCommand { get; }
    public async Task OnInitializeAsync()
    {
        ToDoCollection = new AvaloniaInfiniteScrollCollection<ToDoItemViewModel>()
        {
            OnCanLoadMore = () => _canLoadMore,
            OnLoadMore = async () =>
            {
                Status = Loading;
                var poetries = await poetryStorage.GetPoetriesAsync(Expression.Lambda<Func<Poetry, bool>>(
                        Expression.Constant(true),
                        Expression.Parameter(typeof(Poetry), "p")),
                    PoetryCollection.Count, PageSize);
                Status = string.Empty;
                if (poetries.Count < PageSize)
                {
                    _canLoadMore = false;
                    Status = NoMoreResult;
                }
                if (PoetryCollection.Count == 0 && poetries.Count == 0)
                {
                    Status = NoResult;
                }
                return poetries;
            }
        };
        /*if (ToDoCollection.Count!=0)
        {
            ToDoCollection.Clear();
        }
        AllToDoItems = await _todoStorageService.GetAllTodoListAsync();
        foreach (var toDoItem in AllToDoItems)
        {
            ToDoItemViewModel toDoItemViewModel = new ToDoItemViewModel(toDoItem,this);
            if (toDoItemViewModel != null) ToDoCollection.Add(toDoItemViewModel);
        }*/
    }
    //1115
    private readonly IContentNavigationService _contentNavigationService;
    public ICommand AddToDoCommand { get; }

    public void  AddToDo()
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
    }
    public ICommand UpdateToDoItemStatusCommand { get; }
    public async Task UpdateToDoItemStatusAsync()
    {
        Console.WriteLine("UpdateToDoItemStatusAsync");
        Console.WriteLine(ToDo.Status);
        await _toDoListViewModel.SetToDoItemFinishStatusAsync(this);
    }
    
}