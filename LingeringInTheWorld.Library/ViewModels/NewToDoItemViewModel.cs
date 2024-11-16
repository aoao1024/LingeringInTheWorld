using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;

namespace LingeringInTheWorld.Library.ViewModels;

public class NewToDoItemViewModel :ViewModelBase
{
    private ITodoStorageService _todoStorageService;

    public NewToDoItemViewModel(ITodoStorageService todoStorageService)
    {
        _todoStorageService = todoStorageService;
        AddNewToDoItemCommand = new AsyncRelayCommand(AddNewToDoItemATask);
        ToDo = new ToDo();
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
    }
}