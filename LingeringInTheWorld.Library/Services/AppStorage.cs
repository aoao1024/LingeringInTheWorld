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
        _preferenceStorage.Set(AppStorageConstant.VersionKey,
            AppStorageConstant.Version);
    }

    
    public async Task InsertDiaryAsync(Diary diary)
    {
        await Connection.InsertAsync(diary);
    }
    
    public async Task<IList<Diary>> ListDiaryAsync()=>
        await Connection.Table<Diary>().ToListAsync();
    
    
    public async Task<IList<Diary>> QueryDiaryAsync(string keyword) => 
        await Connection.Table<Diary>()
            .Where(d => d.Title.Contains(keyword))
            .ToListAsync();
    
    public async Task CloseAsync() => await Connection.CloseAsync();

}

public static class AppStorageConstant
{
    public const int Version = 1;
    
    public const string VersionKey = nameof(AppStorageConstant) + "." + nameof(Version);
    //AppStorageConstant.Version
}