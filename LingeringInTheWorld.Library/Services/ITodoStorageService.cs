using System.Linq.Expressions;
using LingeringInTheWorld.Library.Models;

namespace LingeringInTheWorld.Library.Services;

public interface ITodoStorageService
{
   
    Task<int> AddToDoItemAsync(ToDo toDo);
    /*删除*/
    Task<int> DeleteToDoItemAsync(int id);
    /*修改*/
    Task UpdateToDoItemStatusAsync( int id, bool status);

    Task UpdateToDoItemFinishedTimeAsync(int id, DateTime finishedTime);
    /*查询*/
    Task<IList<ToDo>> GetAllTodoListAsync();
    Task<IList<ToDo>> GetToDoList(Expression<Func<ToDo, bool>> where, int skip, int take);
    Task<ToDo> GetToDoItem(int id);
}