using Moq;
using LingeringInTheWorld.Library.Services;
using Xunit.Abstractions;

namespace LingeringInTheWorld.UnitTest.Services;
public class HeFengWeatherServiceTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public HeFengWeatherServiceTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    // 测试服务器正常返回天气数据
    [Fact]
    public async Task GetWeatherByLocationAsync_RealJsonResponse()
    {
        var alertServiceMock = new Mock<IAlertService>();
        var mockAlertService = alertServiceMock.Object;
        var weatherService = new HeFengWeatherService(mockAlertService);
        var latitude = 39.9042;
        var longitude = 116.4074;
        var result = await weatherService.GetWeatherByLocationAsync(latitude, longitude);
        Assert.NotNull(result);
        // 打印结果，帮助调试
        _testOutputHelper.WriteLine("Actual Response:");
        _testOutputHelper.WriteLine($"Temperature: {result.Temperature}, Condition: {result.Condition}, Wind: {result.Wind}");
        Assert.True(result.Temperature != string.Empty);
        Assert.True(result.Condition != string.Empty);
        Assert.True(result.Wind != string.Empty);
        alertServiceMock.Verify(p => p.AlertAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    // 根据城市名称获取经纬度
    [Fact]
    public async Task GetCoordinatesFromLocation_ReturnsCoordinates()
    {
        var alertServiceMock = new Mock<IAlertService>();
        var mockAlertService = alertServiceMock.Object;
        var weatherService = new HeFengWeatherService(mockAlertService);
        var cityName = "Beijing";
        var result = await weatherService.GetCoordinatesFromLocation(cityName);
        Assert.InRange(result.Latitude, 39.9000, 40.0000);  // 增加误差范围
        Assert.InRange(result.Longitude, 116.4000, 116.5000);  // 增加误差范围
        alertServiceMock.Verify(p => p.AlertAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    // 根据城市名称获取经纬度时，返回无效数据（模拟API返回错误）
    [Fact]
    public async Task GetCoordinatesFromLocation_InvalidLocation_ReturnsZeroCoordinates()
    {
        var alertServiceMock = new Mock<IAlertService>();
        var mockAlertService = alertServiceMock.Object;
        var weatherService = new HeFengWeatherService(mockAlertService);
        var cityName = "-----";
        var result = await weatherService.GetCoordinatesFromLocation(cityName);
        Assert.Equal(0.0, result.Latitude);
        Assert.Equal(0.0, result.Longitude);
        alertServiceMock.Verify(p => p.AlertAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
}

