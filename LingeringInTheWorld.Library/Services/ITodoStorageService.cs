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
    /*查询*/
    Task<IList<ToDo>> GetAllTodoListAsync();
}