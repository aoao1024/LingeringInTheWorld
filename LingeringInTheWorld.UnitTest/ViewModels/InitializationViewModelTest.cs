using LingeringInTheWorld.Library.Services;
using LingeringInTheWorld.Library.ViewModels;
using Moq;

namespace LingeringInTheWorld.UnitTest.ViewModels;

public class InitializationViewModelTest {
    [Fact]
    public async Task OnInitializedAsync_NotInitialized() {
        var AccountingStorageMock = new Mock<IAccountingStorage>();
        AccountingStorageMock.Setup(p => p.IsInitialized).Returns(false);
        var mockAccountingStorage = AccountingStorageMock.Object;

        var rootNavigationServiceMock = new Mock<ICashRootNavigationService>();
        var mockRootNavigationService = rootNavigationServiceMock.Object;

        var menuNavigationServiceMock = new Mock<ICashMenuNavigationService>();
        var mockMenuNavigationService = menuNavigationServiceMock.Object;

        var initializationViewModel = new CashInitializationViewModel(
            mockAccountingStorage, mockRootNavigationService, mockMenuNavigationService);

        await initializationViewModel.OnInitializedAsync();
        AccountingStorageMock.Verify(p => p.IsInitialized, Times.Once);
        AccountingStorageMock.Verify(p => p.InitializeAsync(), Times.Once);
        rootNavigationServiceMock.Verify(
            p => p.NavigateTo(CashRootNavigationConstant.MainView), Times.Once);
        menuNavigationServiceMock.Verify(
            p => p.NavigateTo(CashMenuNavigationConstant.MonthView,null), Times.Once);
    }

    [Fact]
    public async Task OnInitializedAsync_Initialized() {
        var AccountingStorageMock = new Mock<IAccountingStorage>();
        AccountingStorageMock.Setup(p => p.IsInitialized).Returns(true);
        var mockAccountingStorage = AccountingStorageMock.Object;

        var rootNavigationServiceMock = new Mock<ICashRootNavigationService>();
        var mockRootNavigationService = rootNavigationServiceMock.Object;

        var menuNavigationServiceMock = new Mock<ICashMenuNavigationService>();
        var mockMenuNavigationService = menuNavigationServiceMock.Object;


        var initializationViewModel = new CashInitializationViewModel(
            mockAccountingStorage, mockRootNavigationService, mockMenuNavigationService);

        await initializationViewModel.OnInitializedAsync();
        AccountingStorageMock.Verify(p => p.IsInitialized, Times.Once);
        AccountingStorageMock.Verify(p => p.InitializeAsync(), Times.Never);
        rootNavigationServiceMock.Verify(
            p => p.NavigateTo(CashRootNavigationConstant.MainView), Times.Once);
        menuNavigationServiceMock.Verify(
            p => p.NavigateTo(CashMenuNavigationConstant.MonthView, null), Times.Once);
    }
}