using System.Linq.Expressions;
using LingeringInTheWorld.Library.Services;
using LingeringInTheWorld.Library.Helpers;
using LingeringInTheWorld.Library.Models;
using SQLite;

namespace LingeringInTheWorld.Library.Services;

public class ExpectedExpensesStorage : IExpectedExpensesStorage
{
    public const string DbName = "Accountingdb.sqlite3";

    public static readonly string ExpectedExpensesDbPath =
        PathHelper.GetLocalFilePath(DbName);

    private SQLiteAsyncConnection _connection;

    public event EventHandler<ExpectedExpensesStorageUpdatedEventArgs>? Updated;

    private SQLiteAsyncConnection Connection =>
        _connection ??= new SQLiteAsyncConnection(ExpectedExpensesDbPath);


    public ExpectedExpensesStorage() {
    }

    public async Task<ExpectedExpenses> GetExpectedExpenses(int id) =>
       await Connection.Table<ExpectedExpenses>().FirstOrDefaultAsync(p => p.Id == id);

    public async Task<ExpectedExpenses> GetMonthExpectedExpensesAsync(int year, int month)
    {
        var model = await
            Connection.Table<ExpectedExpenses>().FirstOrDefaultAsync(p => p.Year == year && p.Month == month);
        return model;
    }

    public async Task<IList<ExpectedExpenses>> GetExpectedExpensesAsync(
        Expression<Func<ExpectedExpenses, bool>> where, int skip, int take) =>
        await Connection.Table<ExpectedExpenses>().Where(where).Skip(skip).Take(take)
            .ToListAsync();

    public async Task CloseAsync() => await Connection.CloseAsync();

    public async Task SaveExpectedExpensesAsync(ExpectedExpenses ExpectedExpenses)
    {
        if (ExpectedExpenses.Id == 0)
        {
            await Connection.InsertAsync(ExpectedExpenses);
        }
        else
        {
            await Connection.UpdateAsync(ExpectedExpenses);
        }

        Updated?.Invoke(this, new ExpectedExpensesStorageUpdatedEventArgs(ExpectedExpenses));
    }

    public async Task InitializeAsync()
    {
        await Connection.CreateTableAsync<ExpectedExpenses>();
    }
}