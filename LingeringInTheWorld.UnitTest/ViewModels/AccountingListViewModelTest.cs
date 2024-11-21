using System.Linq.Expressions;
using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;
using LingeringInTheWorld.Library.ViewModels;
using LingeringInTheWorld.UnitTest.Helpers;
using Moq;

namespace LingeringInTheWorld.UnitTest.ViewModels;

public class AccountingListViewModelTest {

    [Fact]
    public async Task AccountingCollection_Default() {
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        var mockPreferenceStorage = preferenceStorageMock.Object;
        var AccountingStorage = new AccountingStorage(mockPreferenceStorage);

        var resultViewModel = new AccountingListViewModel(AccountingStorage, null);
        var statusList = new List<string>();
        resultViewModel.PropertyChanged += (sender, args) => {
            if (args.PropertyName == nameof(resultViewModel.Status)) {
                statusList.Add(resultViewModel.Status);
            }
        };

        Assert.Equal(0, resultViewModel.AccountingCollection.Count);
        await resultViewModel.OnLoadedAsync();

        Assert.True(resultViewModel.AccountingCollection.Count > 0);

        await AccountingStorage.CloseAsync();
    }

    [Fact]
    public void ShowAccounting_Default() {
        var contentNavigationServiceMock =
            new Mock<ICashContentNavigationService>();
        var mockContentNavigationService =
            contentNavigationServiceMock.Object;

        var AccountingToTap = new Accounting();
        var resultViewModel =
            new AccountingListViewModel(null, mockContentNavigationService);
        resultViewModel.ShowAccounting(AccountingToTap);
        contentNavigationServiceMock.Verify(
            p => p.NavigateTo(CashContentNavigationConstant.DetailView, AccountingToTap), Times.Once);
    }
}