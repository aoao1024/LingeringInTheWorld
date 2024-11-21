using System.Linq.Expressions;
using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;
using LingeringInTheWorld.UnitTest.Helpers;
using Moq;
using static LingeringInTheWorld.Library.Services.AccountingStorageConstant;

namespace LingeringInTheWorld.UnitTest.Services;

public class AccountingStorageTest
{
    [Fact]
    public void IsInitialized_Default() {
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        preferenceStorageMock.Setup(p => p.Get(VersionKey, default(int)))
            .Returns(AccountingStorageConstant.Version);
        var mockPreferenceStorage = preferenceStorageMock.Object;

        var AccountingStorage = new AccountingStorage(mockPreferenceStorage);
        Assert.True(AccountingStorage.IsInitialized);

        preferenceStorageMock.Verify(p => p.Get(VersionKey, default(int)),
            Times.Once);
    }

    [Fact]
    public async Task InitializeAsync_Default() {
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        var mockPreferenceStorage = preferenceStorageMock.Object;

        var AccountingStorage = new AccountingStorage(mockPreferenceStorage);


        await AccountingStorage.InitializeAsync();
        Assert.True(File.Exists(AccountingStorage.AccountingDbPath));

        preferenceStorageMock.Verify(
            p => p.Set(VersionKey, AccountingStorageConstant.Version), Times.Once);
    }

    [Fact]
    public async Task GetaccountingAsync_Default() {
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        var mockPreferenceStorage = preferenceStorageMock.Object;
        var AccountingStorage = new AccountingStorage(mockPreferenceStorage);

        var accounting = await AccountingStorage.GetAccounting(1);
        Assert.True(accounting.Amount > 0);
        await AccountingStorage.CloseAsync();
    }

    [Fact]
    public async Task GetPoetriesAsync_Default() {
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        var mockPreferenceStorage = preferenceStorageMock.Object;
        var AccountingStorage = new AccountingStorage(mockPreferenceStorage);

        var poetries = await AccountingStorage.GetAccountingAsync(null, "");
        Assert.True(poetries.Count() > 0);
        await AccountingStorage.CloseAsync();
    }

    [Fact]
    public async Task GetMonthAccountingAsync_Default()
    {
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
             var mockPreferenceStorage = preferenceStorageMock.Object;
        var AccountingStorage = new AccountingStorage(mockPreferenceStorage);

        var poetries = await AccountingStorage.GetMonthAccountingAsync(DateTime.Now);
        Assert.True(poetries.Count() > 0);
        await AccountingStorage.CloseAsync();
    }
}