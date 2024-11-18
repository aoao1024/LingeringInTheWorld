using System.Threading.Tasks;
using LingeringInTheWorld.Library.Services;
using Ursa.Controls;

namespace LingeringInTheWorld.Services;

//IAlertService接口的实现
public class AlertService : IAlertService {
    //弹出提示框
    public async Task AlertAsync(string title, string message) =>
        await MessageBox.ShowAsync(message, title, button: MessageBoxButton.OK);
    
    // 弹出确认框，返回一个布尔值表示用户是否点击“是”
    public async Task<bool> ConfirmAsync(string title, string message)
    {
        var result = await MessageBox.ShowAsync(message, title, button: MessageBoxButton.YesNo);
        return result == MessageBoxResult.Yes;
    }
}