using LingeringInTheWorld.Library.Services;
using Moq;

namespace LingeringInTheWorld.UnitTest.Helpers;

public static class AppStorageHelper
{
    // 删除数据库文件
    public static void RemoveDatabaseFile() =>
        File.Delete(AppStorage.AppDbPath);

    // 获取已初始化的 AppStorage 实例
    public static async Task<AppStorage> GetInitializedAppStorage()
    {
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        preferenceStorageMock.Setup(p =>
                p.Get(AppStorageConstant.VersionKey, default(int)))
            .Returns(AppStorageConstant.Version);
        var mockPreferenceStorage = preferenceStorageMock.Object;
        var appStorage = new AppStorage(mockPreferenceStorage);
        await appStorage.InitializeAsync();
        return appStorage;
    }
}
