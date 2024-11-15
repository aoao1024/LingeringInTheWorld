using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;

namespace LingeringInTheWorld.Library.ViewModels;

public class TodoListViewModel : ViewModelBase
{
    public ObservableCollection<ToDoItemViewModel> ToDoCollection
    { get; }

    private ITodoStorageService _todoStorageService;
    private IList<ToDo> AllToDoItems;
    
    public  TodoListViewModel(ITodoStorageService todoStorageService)
    {
        _todoStorageService = todoStorageService;
        ToDoCollection = new ObservableCollection<ToDoItemViewModel>();
        OnInitializeCommand=new AsyncRelayCommand(OnInitializeAsync);
    }
    public ICommand OnInitializeCommand { get; }

    public async Task OnInitializeAsync()
    {
        AllToDoItems = await _todoStorageService.GetAllTodoListAsync();
        foreach (var toDoItem in AllToDoItems)
        {
            ToDoItemViewModel toDoItemViewModel = new ToDoItemViewModel(toDoItem);
            if (toDoItemViewModel != null) ToDoCollection.Add(toDoItemViewModel);
        }
    }
}
public class ToDoItemViewModel :ObservableObject
{
    private readonly TodoListViewModel _todoListViewModel;
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