using System.Linq.Expressions;
using LingeringInTheWorld.Library.Models;

namespace LingeringInTheWorld.Library.Services;

public class TodoStorageService : ITodoStorageService
{
    private IToDoStorage _toDoStorage;

    public TodoStorageService(IToDoStorage toDoStorage)
    {
        _toDoStorage = toDoStorage;
        if (!toDoStorage.IsInitialized) toDoStorage.InitializeAsync();
    }
    
    public async Task<int> AddToDoItemAsync(ToDo toDo)
        =>await _toDoStorage.AddToDoItemAsync(toDo);

    public async Task<int> DeleteToDoItemAsync(int id)
        => await _toDoStorage.DeleteToDoItemAsync(id);

    public async Task UpdateToDoItemAsync(int id, object deadline = null, object title = null, object content = null)
    =>await _toDoStorage.UpdateToDoItemAsync(id,deadline,title,content);

    public async Task<IList<ToDo>> GetAllTodoListAsync()
        => await _toDoStorage.GetTodoListAsync(todo => true, 0, int.MaxValue);

}