using System.Linq.Expressions;
using System.Security.Principal;
using LingeringInTheWorld.Library.Models;

namespace LingeringInTheWorld.Library.Services;

public interface IAccountingStorage
{
    bool IsInitialized { get; }
    Task InitializeAsync();

    Task<Accounting> GetAccounting(int id);

    Task<IList<Accounting>> GetAccountingAsync(DateTimeOffset? month, string type);

    Task<IList<Accounting>> GetMonthAccountingAsync(DateTime today);

    Task SaveAccountingAsync(Accounting accounting);

    event EventHandler<AccountingStorageUpdatedEventArgs> Updated;
}

public class AccountingStorageUpdatedEventArgs : EventArgs
{
    public Accounting UpdatedAccounting { get; }

    public AccountingStorageUpdatedEventArgs(Accounting favorite)
    {
        UpdatedAccounting = favorite;
    }
}