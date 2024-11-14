using System.Linq.Expressions;
using LingeringInTheWorld.Library.Helpers;
using LingeringInTheWorld.Library.Models;
using SQLite;
namespace LingeringInTheWorld.Library.Services;
public class ToDoStorage : IToDoStorage
{
    public const string DbName = "todo.sqlite3";
    public static readonly string TodoDbPath = PathHelper.GetLocalFilePath(DbName);
    private SQLiteAsyncConnection _connection;
    //public bool IsInitialized => File.Exists(TodoDbPath);
    private SQLiteAsyncConnection Connection =>
        _connection ??= new SQLiteAsyncConnection(TodoDbPath);
    public async Task<int> AddToDoItemAsync(ToDo toDo) 
        => await Connection.InsertAsync(toDo);
    
    public async Task<int> DeleteToDoItemAsync(int id)
    =>await Connection.DeleteAsync<ToDo>(id);

    public async Task<bool> DeleteToDoItemsAsync(int[] ids)
    {
        var result = true;
        await Connection.RunInTransactionAsync(async (transaction) =>
        {
            try
            {
                foreach (var id in ids)
                    await DeleteToDoItemAsync(id);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                result = false;
                transaction.Rollback();
            }
        });
        return result;
    }
    public async Task UpdateToDoItemAsync(int id, object deadline=null, object title = null, object content = null)
    {
        var toDo =await Connection.FindAsync<ToDo>(id);
        if (deadline!=null)
        {
            toDo.DeadLine = (DateTime)deadline;
        }

        if (title != null) toDo.Title = (string)title;
        if (content != null) toDo.Content = (string)content;
        await Connection.UpdateAsync(toDo);
    }

    public async Task<IList<ToDo>> GetTodoListAsync(Expression<Func<ToDo, bool>> where, int skip, int take)
        => await Connection.Table<ToDo>().Where(where).Skip(skip).Take(take).ToListAsync();

}