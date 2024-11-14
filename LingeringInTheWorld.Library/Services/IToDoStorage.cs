using System.Linq.Expressions;
using LingeringInTheWorld.Library.Models;

namespace LingeringInTheWorld.Library.Services;

public interface IToDoStorage
{
    
    //bool IsInitialized { get; }
    /*没有就创建一个数据库表*/
    //Task<bool> InitializeAsync();
    /*增加*/
    Task<int> AddToDoItemAsync(ToDo toDo);
    /*删除*/
    Task<int> DeleteToDoItemAsync(int id);
    /*修改*/
    Task UpdateToDoItemAsync( int id, object deadline=null,object title = null, object content = null);
    /*查询*/
    Task<IList<ToDo>> GetTodoListAsync(
        Expression<Func<ToDo, bool>> where, int skip, int take);
    
}