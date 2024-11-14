using System.Threading.Tasks;
using LingeringInTheWorld.Library.Services;
using Ursa.Controls;

namespace LingeringInTheWorld.Services;

//IAlertService接口的实现
public class AlertService : IAlertService {
    public async Task AlertAsync(string title, string message) =>
        await MessageBox.ShowAsync(message, title, button: MessageBoxButton.OK);
}