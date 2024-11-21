using System.Linq.Expressions;
using System.Security.Principal;
using LingeringInTheWorld.Library.Models;

namespace LingeringInTheWorld.Library.Services;

public interface IExpectedExpensesStorage
{

    Task<ExpectedExpenses> GetExpectedExpenses(int id);

    Task<ExpectedExpenses> GetMonthExpectedExpensesAsync(int year, int month);

    Task<IList<ExpectedExpenses>> GetExpectedExpensesAsync(
        Expression<Func<ExpectedExpenses, bool>> where, int skip, int take);

    Task SaveExpectedExpensesAsync(ExpectedExpenses ExpectedExpenses);

    event EventHandler<ExpectedExpensesStorageUpdatedEventArgs> Updated;
}

public class ExpectedExpensesStorageUpdatedEventArgs : EventArgs
{
    public ExpectedExpenses UpdatedExpectedExpenses { get; }

    public ExpectedExpensesStorageUpdatedEventArgs(ExpectedExpenses favorite)
    {
        UpdatedExpectedExpenses = favorite;
    }
}