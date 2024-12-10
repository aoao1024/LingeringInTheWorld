using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;

namespace LingeringInTheWorld.Library.ViewModels;

public class NewToDoItemViewModel :ViewModelBase
{
    private ITodoStorageService _todoStorageService;
    private IMenuNavigationService _menuNavigationService;

    public NewToDoItemViewModel(ITodoStorageService todoStorageService,IMenuNavigationService menuNavigationService)
    {
        _todoStorageService = todoStorageService;
        _menuNavigationService = menuNavigationService;
        AddNewToDoItemCommand = new AsyncRelayCommand(AddNewToDoItemATask);
        ToDo = new ToDo();
        // 给 DeadLine 设置一个默认值
        _toDo.CreatedTime = DateTime.Now;  // 设置为当前时间
    }

    public ToDo ToDo
    {
        get => _toDo;
        set => SetProperty(ref _toDo, value);
    } 
    private ToDo _toDo;
    public ICommand AddNewToDoItemCommand { get; }

    public async Task AddNewToDoItemATask()
    {
        await _todoStorageService.AddToDoItemAsync(ToDo);
        ToDo = new ToDo();
        _menuNavigationService.NavigateTo(MenuNavigationConstant.ToDoListView);
    }
}