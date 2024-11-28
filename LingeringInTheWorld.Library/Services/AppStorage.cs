using System.Linq.Expressions;
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
    
    private readonly IPreferenceStorage _preferenceStorage;
    
    public AppStorage(IPreferenceStorage preferenceStorage) {
        _preferenceStorage = preferenceStorage;
    }

    public bool IsInitialized =>
        _preferenceStorage.Get(AppStorageConstant.VersionKey,
            default(int)) == AppStorageConstant.Version;

    public async Task InitializeAsync()
    {
        await Connection.CreateTableAsync<Diary>();
        await Connection.CreateTableAsync<ToDo>();
        _preferenceStorage.Set(AppStorageConstant.VersionKey,
            AppStorageConstant.Version);
    }

    //插入日记
    public async Task InsertDiaryAsync(Diary diary)
    {
        await Connection.InsertAsync(diary);
        Updated?.Invoke(this, diary);
    }

    //删除日记
    public async Task DeleteDiaryAsync(int id)
    {
        await Connection.DeleteAsync<Diary>(id);
    }

    //列出所有日记
    public async Task<IList<Diary>> ListDiaryAsync()=>
        await Connection.Table<Diary>().OrderByDescending(d => d.DateTime)
            .ToListAsync();
    
    //根据ID查询日记
    public async Task<Diary> QueryDiaryByIdAsync(int id)
    {
        return await Connection.Table<Diary>().Where(d => d.Id == id)
            .FirstOrDefaultAsync();
    }

    //根据标题查询日记
    public async Task<IList<Diary>> QueryDiaryByTitleAsync(string title) => 
        await Connection.Table<Diary>()
            .Where(d => d.Title.Contains(title))
            .OrderByDescending(d => d.DateTime)
            .ToListAsync();
    
    public async Task<List<Diary>> GetDiariesAsync(
    Expression<Func<Diary, bool>> where, int skip, int take) =>
    await Connection.Table<Diary>().Where(where).OrderByDescending(d => d.DateTime)  // 按时间降序排列.Skip(skip).Take(take)
        .Skip(skip).Take(take).ToListAsync();
    
    public event EventHandler<Diary> Updated;
    
    public async Task CloseAsync() => await Connection.CloseAsync();

}

public static class AppStorageConstant
{
    public const int Version = 1;
    
    public const string VersionKey = nameof(AppStorageConstant) + "." + nameof(Version);
    //AppStorageConstant.Version
}