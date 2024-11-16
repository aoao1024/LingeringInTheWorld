using System.Linq.Expressions;
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
    Task UpdateToDoItemAsync( int id, object deadline=null,object title = null, object content = null);
    /*查询*/
    Task<IList<ToDo>> GetTodoListAsync(
        Expression<Func<ToDo, bool>> where, int skip, int  take);
    
}