using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;

namespace LingeringInTheWorld.Library.ViewModels;

public class ToDoDetailViewModel :ViewModelBase
{
    private IToDoStorage _todoStorage;
    public ToDoDetailViewModel(IToDoStorage toDoStorage)
    {
        _todoStorage = toDoStorage;
        SubmitCommand = new AsyncRelayCommand(SubmitAsync);
        ToDo = new ToDo();
    }
    public ToDo ToDo
    {
        get => _toDo;
        set => SetProperty(ref _toDo, value);
    } 
    public override void SetParameter(object parameter) {
        ToDo = parameter as ToDo;
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

    public async Task SubmitAsync()
    {
        await _todoStorage.UpdateToDoItemAsync(_toDo.Id,_toDo.DeadLine,_toDo.Title,_toDo.Content,_toDo.Status);
    }

}