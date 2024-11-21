using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;
using LingeringInTheWorld.Library.ViewModels;
using LingeringInTheWorld.Services;
using Moq;

namespace LingeringInTheWorld.UnitTest.ViewModels;

public class DetailViewModelTest {
    [Fact]
    public async Task OnLoadedAsync_Default() {
        var accountingToReturn = new Accounting {
            Id = 1,
            Type = "支出",
            Category = "餐饮",
            Time = DateTime.Now,
            Amount = 12,
            Content = "午饭"
        };

        var accountStorageMock = new Mock<IAccountingStorage>();
        accountStorageMock
            .Setup(p => p.GetAccounting(1))
            .ReturnsAsync(accountingToReturn);
        var mockAccountStorage = accountStorageMock.Object;

        var alertService = new AlertService();

        var detailViewModel = new DetailViewModel(mockAccountStorage, alertService) ;
        detailViewModel.SetParameter(accountingToReturn);

        var loadingList = new List<bool>();
        detailViewModel.PropertyChanged += (sender, args) => {
            if (args.PropertyName == nameof(DetailViewModel.IsLoading)) {
                loadingList.Add(detailViewModel.IsLoading);
            }
        };

        await detailViewModel.OnLoadedAsync();
        Assert.True(detailViewModel.Accounting != null);
    }
}