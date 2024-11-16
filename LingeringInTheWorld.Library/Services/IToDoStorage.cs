using System.Linq.Expressions;
using DynamicData.Alias;
using LingeringInTheWorld.Library.Models;

namespace LingeringInTheWorld.Library.Services;

public interface IToDoStorage
{
    
   
    /*增加*/
    /*bool IsInitialized { get; }
    Task InitializeAsync();*/
    Task<int> AddToDoItemAsync(ToDo toDo);
    /*删除*/
    Task<int> DeleteToDoItemAsync(int id);
    /*修改*/
    Task UpdateToDoItemAsync(int id, DateTime? deadline, string title, string content, bool status);
    /*查询*/
    Task<IList<ToDo>> GetTodoListAsync(
        Expression<Func<ToDo, bool>> where, int skip, int  take);
    
}