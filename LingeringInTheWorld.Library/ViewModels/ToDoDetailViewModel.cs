using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;

namespace LingeringInTheWorld.Library.ViewModels;

public class ToDoDetailViewModel :ViewModelBase
{
    private IToDoStorage _toDoStorage;
    public ToDoDetailViewModel(IToDoStorage toDoStorage)
    {
        _toDoStorage = toDoStorage;
        SubmitCommand = new AsyncRelayCommand(Submit);
        
    }

    public override void SetParameter(object parameter) {
        if (parameter is not Models.ToDo toDo) {
            return;
        }

        ToDo = toDo;
    }

    public ToDo ToDo {
        get => _toDo;
        set => SetProperty(ref _toDo, value);
    }

    private ToDo _toDo;
    
    // private bool _isLoading;
    //
    // public bool IsLoading {
    //     get => _isLoading;
    //     set => SetProperty(ref _isLoading, value);
    // }

    // public ICommand OnLoadedCommand { get; }

    //确定按钮的点击事件  带不带参数呢？好像不带？
    public ICommand SubmitCommand { get; }

    public async Task Submit()
    {
        
        await _toDoStorage.AddToDoItemAsync(ToDo);

    }

}