using System.Linq.Expressions;
using System.Reflection;
using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;
using LingeringInTheWorld.Library.ViewModels;
using Moq;

namespace LingeringInTheWorld.UnitTest.ViewModels;

public class DiaryViewModelTest
{
    private readonly Mock<IAppStorage> _mockAppStorage;
    private readonly Mock<IAlertService> _mockAlertService;
    private readonly Mock<IContentNavigationService> _mockContentNavigationService;
    private readonly DiaryViewModel _viewModel;

    public DiaryViewModelTest()
    {
        _mockAppStorage = new Mock<IAppStorage>();
        _mockAlertService = new Mock<IAlertService>();
        _mockContentNavigationService = new Mock<IContentNavigationService>();
        _viewModel = new DiaryViewModel(_mockAppStorage.Object, _mockAlertService.Object, _mockContentNavigationService.Object);
    }
    
    // 测试：验证 AddDiaryCommand 是否调用导航服务
    [Fact]
    public void AddDiaryCommand_NavigatesToAddDiaryView()
    {
        // Act
        // 执行添加日记命令
        _viewModel.AddDiaryCommand.Execute(null);

        // Assert
        // 验证导航服务是否被调用
        _mockContentNavigationService.Verify(service => 
            service.NavigateTo(ContentNavigationConstant.DiaryAddView,It.IsAny<object>()), 
            Times.Once);
    }

    // 测试：验证 DeleteDiaryCommand 是否删除日记并从集合中移除
    [Fact]
    public Task DeleteDiaryCommand_DeletesDiaryAndRemovesFromCollection()
    {
        // Arrange
        var diary = new Diary { Id = 1, Title = "Test Diary" };
        _mockAppStorage.Setup(m => m.QueryDiaryByIdAsync(diary.Id)).ReturnsAsync(diary);
        _mockAppStorage.Setup(m => m.DeleteDiaryAsync(diary.Id)).Returns(Task.CompletedTask);  // Mock DeleteDiaryAsync

        // 模拟 ConfirmAsync 返回 true，表示用户确认删除
        _mockAlertService.Setup(service => service.ConfirmAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        // Act
        // 执行删除命令
        _viewModel.DeleteDiaryCommand.Execute(diary);

        // Assert
        // 验证 DeleteDiaryAsync 被调用
        _mockAppStorage.Verify(m => m.DeleteDiaryAsync(diary.Id), Times.Once);

        // 验证日记已从 DiaryCollection 中移除
        Assert.DoesNotContain(diary, _viewModel.DiaryCollection);
        return Task.CompletedTask;
    }


    // 测试：验证 OnLoadMore 是否正确加载更多日记
    [Fact]
    public async Task OnLoadMore_LoadsMoreDiaries()
    {
        // Arrange
        var mockDiaries = new List<Diary>
        {
            new Diary { Id = 1, Title = "Diary 1" },
            new Diary { Id = 2, Title = "Diary 2" }
        };

        _mockAppStorage.Setup(m => m.GetDiariesAsync(It.IsAny<Expression<Func<Diary, bool>>>(), 0, 20))
            .ReturnsAsync(mockDiaries);

        // Act
        // 调用 OnLoadMore 方法
        var diaries = await _viewModel.DiaryCollection.OnLoadMore();

        // Assert
        // 验证返回的日记数量是否正确
        var enumerable = diaries as Diary[] ?? diaries.ToArray();
        Assert.Equal(mockDiaries.Count, enumerable.Count());
        Assert.Equal(mockDiaries[0], enumerable.FirstOrDefault());
    }

    // 测试：验证 ShowDiaryDetailCommand 是否正确导航到日记详情页
    [Fact]
    public void ShowDiaryDetailCommand_NavigatesToDiaryDetailView()
    {
        // Arrange
        var diary = new Diary { Id = 1, Title = "Test Diary" };

        // Act
        // 执行显示日记详情命令
        _viewModel.ShowDiaryDetailCommand.Execute(diary);

        // Assert
        // 验证导航服务是否被调用
        _mockContentNavigationService.Verify(service => 
            service.NavigateTo(ContentNavigationConstant.DiaryDetailView, diary), 
            Times.Once);
    }

    // 测试：验证 AppStorageOnUpdated 事件处理器是否正常工作
    [Fact]
    public Task AppStorageOnUpdated_UpdatesDiaryCollection()
    {
        // Arrange
        var diary = new Diary { Id = 1, Title = "Updated Diary" };

        // 模拟日记在存储中存在
        _mockAppStorage.Setup(m => m.QueryDiaryByIdAsync(diary.Id)).ReturnsAsync(diary);

        // Act
        // 使用反射调用 private 方法
        var method = typeof(DiaryViewModel).GetMethod("AppStorageOnUpdated", BindingFlags.NonPublic | BindingFlags.Instance);
        method?.Invoke(_viewModel, new object[] { this, diary });

        // Assert
        // 验证该日记是否被插入到 DiaryCollection 中
        Assert.Contains(diary, _viewModel.DiaryCollection);
        return Task.CompletedTask;
    }
}

