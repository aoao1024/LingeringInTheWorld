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
        SubmitCommand = new RelayCommand<ToDo>(Submit);
        
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

    //确定按钮的点击事件
    public IRelayCommand<ToDo> SubmitCommand;

    public void  Submit(ToDo toDo)
    {
        //更新
        _toDoStorage.UpdateToDoItemAsync(ToDo.Id,ToDo.DeadLine,ToDo.Title,ToDo.Content);
    }

}