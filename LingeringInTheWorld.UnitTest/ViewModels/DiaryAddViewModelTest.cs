using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;
using LingeringInTheWorld.Library.ViewModels;
using Moq;

namespace LingeringInTheWorld.UnitTest.ViewModels;

public class DiaryAddViewModelTest
{
    private readonly Mock<IAppStorage> _mockAppStorage;
    private readonly Mock<IAlertService> _mockAlertService;
    private readonly Mock<IWeatherService> _mockWeatherService;
    private readonly Mock<ILocationService> _mockLocationService;
    private readonly DiaryAddViewModel _viewModel;

    public DiaryAddViewModelTest()
    {
        // 初始化 Moq 对象
        _mockAppStorage = new Mock<IAppStorage>();
        _mockAlertService = new Mock<IAlertService>();
        _mockWeatherService = new Mock<IWeatherService>();
        _mockLocationService = new Mock<ILocationService>();
        Mock<IMenuNavigationService> menuNevigationService = new();
        var menuViewModel = new MenuViewModel(menuNevigationService.Object);
        
        _viewModel = new DiaryAddViewModel(
            _mockAppStorage.Object, 
            _mockAlertService.Object, 
            _mockWeatherService.Object, 
            _mockLocationService.Object, 
            menuViewModel
            );
    }

    // 测试：验证 OnInitialize 是否设置了当前时间和位置
    [Fact]
    public Task OnInitialize_ShouldSetCurrentTimeAndLocation()
    {
        // Arrange: 模拟返回的位置和天气数据
        _mockLocationService.Setup(s => s.GetCurrentLocationAsync())
            .ReturnsAsync((1.0, 2.0, "Test Location"));

        _mockWeatherService.Setup(s => s.GetWeatherByLocationAsync(1.0, 2.0))
            .ReturnsAsync(new WeatherInfo { Condition = "Sunny" });

        // Act: 调用 OnInitialize 方法
        _viewModel.OnInitializeCommand.Execute(null);

        // Assert: 确保当前时间已设置，并且位置和天气信息已获取
        Assert.NotNull(_viewModel.CurrentTime);
        Assert.Equal("Test Location", _viewModel.CurrentLocation);
        Assert.Equal("Sunny", _viewModel.CurrentWeatherCondition);
        return Task.CompletedTask;
    }

    // 测试：验证 SaveDiaryAsync 是否保存日记，当用户确认时
    [Fact]
    public Task SaveDiaryAsync_ShouldSaveDiary_WhenUserConfirms()
    {
        // Arrange: 设置模拟返回的确认框为 "Yes"
        _mockAlertService.Setup(s => s.ConfirmAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        
        // 模拟用户输入
        _viewModel.Title = "Test Title";
        _viewModel.Content = "Test Content";
        _viewModel.CurrentLocation = "Test Location";
        _viewModel.Tags.Add("Test Tag");

        // 模拟数据库保存
        _mockAppStorage.Setup(s => s.InsertDiaryAsync(It.IsAny<Diary>())).Returns(Task.CompletedTask);
        _mockAppStorage.Setup(s => s.QueryDiaryByIdAsync(It.IsAny<int>())).ReturnsAsync(new Diary());

        // Act: 执行保存日记操作
        _viewModel.SaveDiaryCommand.Execute(null);

        // Assert: 确保执行了保存日记的方法，并且成功
        _mockAppStorage.Verify(s => s.InsertDiaryAsync(It.IsAny<Diary>()), Times.Once);
        _mockAlertService.Verify(s => s.AlertAsync("保存成功", "日记已成功保存！"), Times.Once);
        return Task.CompletedTask;
    }

    // 测试：验证 SaveDiaryAsync 是否保存日记，当用户取消时
    [Fact]
    public Task SaveDiaryAsync_ShouldNotSaveDiary_WhenUserCancels()
    {
        // Arrange: 设置模拟返回的确认框为 "No"
        _mockAlertService.Setup(s => s.ConfirmAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        // 模拟用户输入
        _viewModel.Title = "Test Title";
        _viewModel.Content = "Test Content";

        // Act: 执行保存日记操作
        _viewModel.SaveDiaryCommand.Execute(null);

        // Assert: 确保没有调用保存方法
        _mockAppStorage.Verify(s => s.InsertDiaryAsync(It.IsAny<Diary>()), Times.Never);
        return Task.CompletedTask;
    }

    // 测试：验证 UpdateLocation 是否更新当前位置
    [Fact]
    public void UpdateLocation_ShouldUpdateCurrentLocation()
    {
        // Act: 设置新的地址
        _viewModel.NewLocation = "New Location";
        _viewModel.UpdateLocationCommand.Execute(null);

        // Assert: 确保 CurrentLocation 已更新
        Assert.Equal("New Location", _viewModel.CurrentLocation);
    }

    // 测试：验证 AddTag 是否添加标签
    [Fact]
    public void AddTag_ShouldAddTagToTagsList()
    {
        // Act: 添加标签
        _viewModel.NewTag = "Tag 1";
        _viewModel.AddTagCommand.Execute(null);

        // Assert: 确保标签已添加
        Assert.Contains("Tag 1", _viewModel.Tags);
    }
}
