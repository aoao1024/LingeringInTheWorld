namespace LingeringInTheWorld.Library.Services;


//AlertService接口 用于弹出提示框
public interface IAlertService {
    //提示框
    Task AlertAsync(string title, string message);
    //确认框
    Task<bool> ConfirmAsync(string title, string message);
}