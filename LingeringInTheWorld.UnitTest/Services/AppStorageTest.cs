using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;
using LingeringInTheWorld.UnitTest.Helpers;
using Moq;

namespace LingeringInTheWorld.UnitTest.Services;
    
public class AppStorageTest : IDisposable
{
    public AppStorageTest()
    {
        // 在每个测试前清理数据库文件
        AppStorageHelper.RemoveDatabaseFile();
    }

    // 测试 IsInitialized 属性
    [Fact]
    public void IsInitialized_Default()
    {
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        preferenceStorageMock.Setup(p =>
            p.Get(AppStorageConstant.VersionKey, default(int)))
            .Returns(AppStorageConstant.Version);
        var mockPreferenceStorage = preferenceStorageMock.Object;
        var appStorage = new AppStorage(mockPreferenceStorage);

        Assert.True(appStorage.IsInitialized);

        preferenceStorageMock.Verify(p =>
            p.Get(AppStorageConstant.VersionKey, default(int)), Times.Once);
    }

    // 测试 InitializeAsync 方法
    [Fact]
    public async Task InitializeAsync_Default()
    {
        // Mock
        var preferenceStorageMock = new Mock<IPreferenceStorage>();
        var mockPreferenceStorage = preferenceStorageMock.Object;
        var appStorage = new AppStorage(mockPreferenceStorage);
        
        Assert.False(File.Exists(AppStorage.AppDbPath)); // 确保数据库文件不存在
        
        await appStorage.InitializeAsync(); // 执行初始化
        
        Assert.True(File.Exists(AppStorage.AppDbPath)); // 确保数据库文件已创建
        preferenceStorageMock.Verify(p =>   
            p.Set(AppStorageConstant.VersionKey, AppStorageConstant.Version), Times.Once);  // 确保版本号已写入
    }

    // 测试 InsertDiaryAsync 方法
    [Fact]
    public async Task InsertDiaryAsync_Default()
    {
        var appStorage = await AppStorageHelper.GetInitializedAppStorage();

        var diary = new Diary { Title = "Test Diary", Content = "This is a test diary." };
        await appStorage.InsertDiaryAsync(diary);

        var insertedDiary = await appStorage.QueryDiaryByIdAsync(diary.Id);
        Assert.NotNull(insertedDiary);
        Assert.Equal(diary.Title, insertedDiary.Title);
        Assert.Equal(diary.Content, insertedDiary.Content);
    }

    // 测试 DeleteDiaryAsync 方法
    [Fact]
    public async Task DeleteDiaryAsync_Default()
    {
        var appStorage = await AppStorageHelper.GetInitializedAppStorage();

        var diary = new Diary { Title = "Test Diary to Delete", Content = "This is a test diary to delete." };
        await appStorage.InsertDiaryAsync(diary);
        var insertedDiary = await appStorage.QueryDiaryByIdAsync(diary.Id);
        Assert.NotNull(insertedDiary);

        await appStorage.DeleteDiaryAsync(diary.Id); // 删除日记
        var deletedDiary = await appStorage.QueryDiaryByIdAsync(diary.Id);
        Assert.Null(deletedDiary); // 确保日记已被删除
    }

    // 测试 ListDiaryAsync 方法
    [Fact]
    public async Task ListDiaryAsync_Default()
    {
        var appStorage = await AppStorageHelper.GetInitializedAppStorage();

        var diary1 = new Diary { Title = "Diary 1", Content = "Content 1" };
        var diary2 = new Diary { Title = "Diary 2", Content = "Content 2" };
        await appStorage.InsertDiaryAsync(diary1);
        await appStorage.InsertDiaryAsync(diary2);

        var diaries = await appStorage.ListDiaryAsync();
        Assert.Equal(2, diaries.Count);
        Assert.Contains(diaries, d => d.Title == "Diary 1");
        Assert.Contains(diaries, d => d.Title == "Diary 2");
    }

    // 测试 QueryDiaryByTitleAsync 方法
    [Fact]
    public async Task QueryDiaryByTitleAsync_Default()
    {
        var appStorage = await AppStorageHelper.GetInitializedAppStorage();

        var diary1 = new Diary { Title = "My First Diary", Content = "This is my first diary." };
        var diary2 = new Diary { Title = "Another Diary", Content = "This is another diary." };
        await appStorage.InsertDiaryAsync(diary1);
        await appStorage.InsertDiaryAsync(diary2);

        var result = await appStorage.QueryDiaryByTitleAsync("First");
        Assert.Single(result); // 只返回匹配的日记
        Assert.Equal("My First Diary", result.First().Title);
    }

    public void Dispose()
    {
        // 确保数据库连接关闭
        var appStorage = AppStorageHelper.GetInitializedAppStorage().Result;
        appStorage.CloseAsync().Wait();
        // 在测试结束后清理数据库文件
        AppStorageHelper.RemoveDatabaseFile();
    }
}

