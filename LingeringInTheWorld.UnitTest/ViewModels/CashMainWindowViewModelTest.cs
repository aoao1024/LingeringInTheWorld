using LingeringInTheWorld.Library.Services;
using LingeringInTheWorld.Library.ViewModels;
using Moq;

namespace LingeringInTheWorld.UnitTest.ViewModels;

public class CashMainWindowViewModelTest {
    [Fact]
    public async Task OnInitializedAsync_NotInitialized() {
        var poetryStorageMock = new Mock<IAccountingStorage>();
        poetryStorageMock.Setup(p => p.IsInitialized).Returns(false);
        var mockPoetryStorage = poetryStorageMock.Object; 

        var rootNavigationServiceMock = new Mock<ICashRootNavigationService>();
        var mockRootNavigationService = rootNavigationServiceMock.Object;
        var menuNavigationServiceMock = new Mock<ICashMenuNavigationService>();
        var mockMenuNavigationService = menuNavigationServiceMock.Object;

        var mainWindowViewModel = new CashMainWindowViewModel(
            mockPoetryStorage, mockRootNavigationService, mockMenuNavigationService);

        mainWindowViewModel.OnInitialized();
        poetryStorageMock.Verify(p => p.IsInitialized, Times.Once);
        rootNavigationServiceMock.Verify(
            p => p.NavigateTo(RootNavigationConstant.InitializationView),
            Times.Once);
        menuNavigationServiceMock.Verify(
            p => p.NavigateTo(CashMenuNavigationConstant.MonthView, null),
            Times.Never);
    }

    [Fact]
    public async Task OnInitializedAsync_Initialized() {
        var poetryStorageMock = new Mock<IAccountingStorage>();
        poetryStorageMock.Setup(p => p.IsInitialized).Returns(true);
        var mockPoetryStorage = poetryStorageMock.Object;

        var rootNavigationServiceMock = new Mock<ICashRootNavigationService>();
        var mockRootNavigationService = rootNavigationServiceMock.Object;
        var menuNavigationServiceMock = new Mock<ICashMenuNavigationService>();
        var mockMenuNavigationService = menuNavigationServiceMock.Object;


        var mainWindowViewModel = new CashMainWindowViewModel(
            mockPoetryStorage, mockRootNavigationService, mockMenuNavigationService);

        mainWindowViewModel.OnInitialized();
        poetryStorageMock.Verify(p => p.IsInitialized, Times.Once);
        rootNavigationServiceMock.Verify(
            p => p.NavigateTo(CashRootNavigationConstant.MainView), Times.Once);
        menuNavigationServiceMock.Verify(
            p => p.NavigateTo(CashMenuNavigationConstant.MonthView, null),
            Times.Once);
    }
}