using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;
using LingeringInTheWorld.Library.ViewModels;
using Moq;

namespace CashBook.UnitTest.ViewModels;

public class MonthViewModelTest {

    [Fact]
    public async Task LoadedCommandFunction_Default() {

        var expectedExpensesReturn = new ExpectedExpenses
        {
            Id = 1,
            Year = 2024,
            Month = 11,
            Value = 2000
        };

        var expectedExpensesStorageMock = new Mock<IExpectedExpensesStorage>();
        expectedExpensesStorageMock.Setup(p => p.GetExpectedExpenses(1))
            .ReturnsAsync(expectedExpensesReturn);
        var mockExpectedExpenseStorage = expectedExpensesStorageMock.Object;

        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        var mockPreferenceStorage = preferenceStorageMock.Object;
        var AccountingStorage = new AccountingStorage(mockPreferenceStorage);
        var service = new MonthStatisticsService(AccountingStorage);

        var monthPageViewModel =
            new MonthViewModel(mockExpectedExpenseStorage, service,
                null);

        var loadingList = new List<bool>();
        monthPageViewModel.PropertyChanged += (sender, args) => {
            if (args.PropertyName == nameof(MonthViewModel.IsLoading)) {
                loadingList.Add(monthPageViewModel.IsLoading);
            }
        };

        Assert.True(monthPageViewModel.MonthStatistics == null);
        await monthPageViewModel.OnLoadedAsync();
        Assert.True(monthPageViewModel.MonthStatistics != null);
    }
}