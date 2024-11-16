using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;

namespace LingeringInTheWorld.Library.ViewModels;

public class ToDoListViewModel : ViewModelBase
{
    public ObservableCollection<ToDoItemViewModel> ToDoCollection
    { get; }

    private ITodoStorageService _todoStorageService;
    private IList<ToDo> AllToDoItems;
    public  ToDoListViewModel(ITodoStorageService todoStorageService,IContentNavigationService contentNavigationService)
    {
        _todoStorageService = todoStorageService;
        _contentNavigationService = contentNavigationService;
        ToDoCollection = new ObservableCollection<ToDoItemViewModel>();
        OnInitializeCommand=new AsyncRelayCommand(OnInitializeAsync);
        AddToDoCommand=new RelayCommand(AddToDo);
    }
    public ICommand OnInitializeCommand { get; }
    public async Task OnInitializeAsync()
    {
        if (ToDoCollection.Count!=0)
        {
            ToDoCollection.Clear();
        }
        AllToDoItems = await _todoStorageService.GetAllTodoListAsync();
        foreach (var toDoItem in AllToDoItems)
        {
            ToDoItemViewModel toDoItemViewModel = new ToDoItemViewModel(toDoItem);
            if (toDoItemViewModel != null) ToDoCollection.Add(toDoItemViewModel);
        }
    }
    //1115
    private readonly IContentNavigationService _contentNavigationService;
    public ICommand AddToDoCommand { get; }

    public void  AddToDo()
    {
        _contentNavigationService.NavigateTo(ContentNavigationConstant.NewToDoItemView);
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
    public ToDoItemViewModel(ToDo todo)
    {
        ToDo = todo;
    }
    
}