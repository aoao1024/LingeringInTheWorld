using System.Linq.Expressions;
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

    // 测试 GetDiariesAsync 方法
    [Fact]
    public async Task GetDiariesAsync_WithConditionsAndPagination()
    {
        var appStorage = await AppStorageHelper.GetInitializedAppStorage();

        // 插入一些测试数据
        var diary1 = new Diary { Title = "Diary 1", Content = "Content 1", DateTime = DateTime.Now.AddDays(-2) };
        var diary2 = new Diary { Title = "Diary 2", Content = "Content 2", DateTime = DateTime.Now.AddDays(-1) };
        var diary3 = new Diary { Title = "Diary 3", Content = "Content 3", DateTime = DateTime.Now };

        await appStorage.InsertDiaryAsync(diary1);
        await appStorage.InsertDiaryAsync(diary2);
        await appStorage.InsertDiaryAsync(diary3);

        // 测试：按日期降序查询所有日记，分页获取前两条记录
        var skip = 0;
        var take = 2;
        Expression<Func<Diary, bool>> whereCondition = d => d.Title.Contains("Diary");
    
        var diaries = await appStorage.GetDiariesAsync(whereCondition, skip, take);

        // 确保返回的数量符合分页要求
        Assert.Equal(take, diaries.Count);

        // 确保结果按时间降序排列
        Assert.True(diaries[0].DateTime > diaries[1].DateTime);

        // 确保返回的日记符合条件
        Assert.Contains(diaries, d => d.Title == "Diary 3");
        Assert.Contains(diaries, d => d.Title == "Diary 2");

        // 测试：分页跳过前1条记录并获取剩余的记录
        skip = 1;
        take = 2;
        diaries = await appStorage.GetDiariesAsync(whereCondition, skip, take);

        // 确保分页功能有效，返回的记录应为从第二条开始的
        Assert.Equal(2, diaries.Count);  // 总共有3条记录，分页查询2条，跳过1条

        // 确保结果按时间降序排列
        Assert.True(diaries[0].DateTime > diaries[1].DateTime);
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

