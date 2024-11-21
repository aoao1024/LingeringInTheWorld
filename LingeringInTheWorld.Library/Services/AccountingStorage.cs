using System.Linq.Expressions;
using LingeringInTheWorld.Library.Helpers;
using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;
using LingeringInTheWorld.Library.Helpers;
using LingeringInTheWorld.Library.Models;
using SQLite;

namespace LingeringInTheWorld.Library.Services;

public class AccountingStorage : IAccountingStorage {
    public const int NumberAccounting = 30;

    public const string DbName = "Accountingdb.sqlite3";

    public static readonly string AccountingDbPath =
        PathHelper.GetLocalFilePath(DbName);

    private SQLiteAsyncConnection _connection;

    public event EventHandler<AccountingStorageUpdatedEventArgs>? Updated;

    private SQLiteAsyncConnection Connection =>
        _connection ??= new SQLiteAsyncConnection(AccountingDbPath);

    private readonly IPreferenceStorage _preferenceStorage;

    public AccountingStorage(IPreferenceStorage preferenceStorage) {
        _preferenceStorage = preferenceStorage;
    }

    public bool IsInitialized =>
        _preferenceStorage.Get(AccountingStorageConstant.VersionKey,
            default(int)) == AccountingStorageConstant.Version;

    public async Task InitializeAsync() {
        await using var dbFileStream =
            new FileStream(AccountingDbPath, FileMode.OpenOrCreate);
        await using var dbAssetStream =
            typeof(AccountingStorage).Assembly.GetManifestResourceStream(DbName) ??
            throw new Exception($"Manifest not found: {DbName}");
        await dbAssetStream.CopyToAsync(dbFileStream);

        _preferenceStorage.Set(AccountingStorageConstant.VersionKey,
            AccountingStorageConstant.Version);
    }

    public async Task<Accounting> GetAccounting(int id) =>
       await Connection.Table<Accounting>().FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IList<Accounting>> GetAccountingAsync(DateTimeOffset? time, string type)
    {
        var query = Connection.Table<Accounting>();
        if (time != null)
        {
            var month = time.Value;
            var start = new DateTime(month.Year, month.Month, 1);
            var end = DateTime.Now;
            if (month.Month == 12)
            {
                end = new DateTime(month.Year, month.Month, 31);
            }
            else
            {
                end = new DateTime(month.Year, month.Month + 1, 1).AddDays(-1);
            }

            query = Connection.Table<Accounting>()
                .Where(x => x.Time >= start && x.Time <= end);
        }
        if (!string.IsNullOrEmpty(type) && type != "所有")
        {
            query = query.Where(x => x.Type == type);
        }

        return await query.ToListAsync();
    }

    public async Task<IList<Accounting>> GetMonthAccountingAsync(DateTime today)
    {
        var start = new DateTime(today.Year, today.Month, 1);
        var end = DateTime.Now;
        if (today.Month == 12)
        {
            end = new DateTime(today.Year, today.Month, 31);
        }
        else
        {
            end = new DateTime(today.Year, today.Month + 1, 1).AddDays(-1);
        }
       
        return await Connection.Table<Accounting>().Where(x => x.Time >= start && x.Time <= end)
            .ToListAsync();
    }

    public async Task CloseAsync() => await Connection.CloseAsync();

    public async Task SaveAccountingAsync(Accounting Accounting)
    {
        if (Accounting.Id == 0)
        {
            await Connection.InsertAsync(Accounting);
        }
        else
        {
            await Connection.UpdateAsync(Accounting);
        }
       
        Updated?.Invoke(this, new AccountingStorageUpdatedEventArgs(Accounting));
    }
}

public static class AccountingStorageConstant {
    public const string VersionKey =
        nameof(AccountingStorageConstant) + "." + nameof(Version);

    public const int Version = 1;
}