namespace LingeringInTheWorld.Library.Services;

public interface ICashMenuNavigationService
{
    void NavigateTo(string view, object parameter = null);
}

public static class CashMenuNavigationConstant {
    public const string MonthView = nameof(MonthView);

    public const string AccountingListView = nameof(AccountingListView);

    public const string DetailView = nameof(DetailView);

    public const string ExpectedExpensesView = nameof(ExpectedExpensesView);
}