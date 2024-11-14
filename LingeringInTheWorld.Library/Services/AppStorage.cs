using LingeringInTheWorld.Library.Helpers;
using LingeringInTheWorld.Library.Models;
using SQLite;

namespace LingeringInTheWorld.Library.Services;

public class AppStorage : IAppStorage
{
    public const string DbName = "appdb.sqlite3";
    
    public static readonly string AppDbPath =
        PathHelper.GetLocalFilePath(DbName);
    
    private SQLiteAsyncConnection _connection;
    private SQLiteAsyncConnection Connection =>
        _connection ??= new SQLiteAsyncConnection(AppDbPath);
    
    
    public async Task InitializeAsync()
    {
        await Connection.CreateTableAsync<Diary>();
    }

    
    public async Task InsertAsync(Diary diary)
    {
        await Connection.InsertAsync(diary);
    }
    
    public async Task<IList<Diary>> ListAsync()=>
        await Connection.Table<Diary>().ToListAsync();
    
    
    public async Task<IList<Diary>> QueryAsync(string keyword) => 
        await Connection.Table<Diary>()
            .Where(d => d.Title.Contains(keyword))
            .ToListAsync();
}