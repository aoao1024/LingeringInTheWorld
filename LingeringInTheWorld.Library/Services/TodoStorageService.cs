using System.Linq.Expressions;
using LingeringInTheWorld.Library.Models;

namespace LingeringInTheWorld.Library.Services;

public class TodoStorageService : ITodoStorageService
{
    private IToDoStorage _toDoStorage;

    public TodoStorageService(IToDoStorage toDoStorage)
    {
        _toDoStorage = toDoStorage;
        //if (!toDoStorage.IsInitialized) toDoStorage.InitializeAsync();
    }
    
    public async Task<int> AddToDoItemAsync(ToDo toDo)
        =>await _toDoStorage.AddToDoItemAsync(toDo);

    public async Task<int> DeleteToDoItemAsync(int id)
        => await _toDoStorage.DeleteToDoItemAsync(id);

    public async Task UpdateToDoItemStatusAsync(int id, bool status)
    {
        _toDoStorage.UpdateToDoItemAsync(id, null, null, null, status);
    }
    

    public async Task<IList<ToDo>> GetAllTodoListAsync()
        => await _toDoStorage.GetTodoListAsync(todo => true, 0, int.MaxValue);

    public async Task<IList<ToDo>> GetToDoList(Expression<Func<ToDo, bool>> where, int skip, int take)
        => await _toDoStorage.GetTodoListAsync(where, skip, take);
}