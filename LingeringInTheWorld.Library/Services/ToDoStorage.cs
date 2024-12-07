using System.Linq.Expressions;
using LingeringInTheWorld.Library.Helpers;
using LingeringInTheWorld.Library.Models;
using SQLite;
namespace LingeringInTheWorld.Library.Services;
public class ToDoStorage : IToDoStorage
{
    public const string DbName = "appdb.sqlite3";
    public static readonly string TodoDbPath = PathHelper.GetLocalFilePath(DbName);
    private SQLiteAsyncConnection _connection;
    private SQLiteAsyncConnection Connection =>
        _connection ??= new SQLiteAsyncConnection(TodoDbPath);
    //对照favorite改的
    private readonly IPreferenceStorage _preferenceStorage;
    public  SQLiteAsyncConnection TestConnection => Connection;
    public ToDoStorage(IPreferenceStorage preferenceStorage) {
        _preferenceStorage = preferenceStorage;
    }
    
    public async Task<int> AddToDoItemAsync(ToDo toDo) 
        => await Connection.InsertAsync(toDo);
    
    public async Task<int> DeleteToDoItemAsync(int id)
    =>await Connection.DeleteAsync<ToDo>(id);
    
    public async Task<bool> UpdateToDoItemAsync(int id, DateTime? deadline, string title, string content, bool status)
    {
        var toDo =await Connection.FindAsync<ToDo>(id);
        if (toDo!=null)
        {
            if (deadline!=null)
            {
                toDo.DeadLine = (DateTime)deadline;
            }
            if (title != null) toDo.Title = (string)title;
            if (content != null) toDo.Content = (string)content;
            if (status) toDo.Status = status;
            int result= await Connection.UpdateAsync(toDo);
            if (result>0)
            {
                return true;
            }
            return false;
        }
            return false;
    }
    
    public async Task<bool> DeleteToDoItemsAsync(int[] ids)
    {
        var result = true;
        await Connection.RunInTransactionAsync( (transaction) =>
        {
            try
            {
                foreach (var id in ids)
                {
                    int midResult = transaction.Execute("DELETE FROM ToDo WHERE Id = ?", id);
                    if (midResult==0)
                    {
                        result = false;
                        throw new Exception($"删除出现错误！！");
                    }
                }
                if (result) transaction.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                transaction.Rollback();
                result = false;
            }
        });
        return result;
    }
    public async Task<IList<ToDo>> GetTodoListAsync(Expression<Func<ToDo, bool>> where, int skip, int take)
        => await Connection.Table<ToDo>().Where(where).Skip(skip).Take(take).ToListAsync();
    
    public async Task<ToDo> GetToDoItemAsync(int id)
        => await Connection.FindAsync<ToDo>(id);

    public async Task CloseAsync()
    {
        await Connection.CloseAsync();
    }
}
