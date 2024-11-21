using System.Windows.Input;
using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;

namespace LingeringInTheWorld.Library.ViewModels;

using CommunityToolkit.Mvvm.Input;

public class DetailViewModel1 : ViewModelBase
{
    private readonly IAccountingStorage _accountingStorage;
    private readonly IAIService _aiService;

    public DetailViewModel1(IAccountingStorage accountingStorage, IAIService aiService)
    {
        _accountingStorage = accountingStorage;
        _aiService = aiService;

        ExtractCommand = new AsyncRelayCommand(ExtractAccountingAsync);
    }

    public string InputText { get; set; } // 用户输入的文本
    public ICommand ExtractCommand { get; }

    private async Task ExtractAccountingAsync()
    {
        if (string.IsNullOrWhiteSpace(InputText))
        {
            return;
        }

        var extracted = await _aiService.ExtractAccounting(InputText);

        // 将提取的数据保存为 Accounting 对象
        var accounting = new Accounting
        {
            Type = extracted.Type,
            Category = extracted.Category,
            Amount = extracted.Amount,
            Time = extracted.Date
        };

        await _accountingStorage.SaveAccountingAsync(accounting);

        // 提示保存成功
    }
}
