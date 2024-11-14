namespace LingeringInTheWorld.Library.Services;


//AlertService接口 用于弹出提示框
public interface IAlertService {
    Task AlertAsync(string title, string message);
}