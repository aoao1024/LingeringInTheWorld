using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;
using System.Collections.ObjectModel;

namespace LingeringInTheWorld.Library.ViewModels;

public class DetailViewModel : ViewModelBase
{
    private IAccountingStorage _accountingStorage;
    private IAlertService _alertService;

    public DetailViewModel(IAccountingStorage accountingStorage, IAlertService alertService)
    {
        _accountingStorage = accountingStorage;
        _alertService = alertService;

        OnLoadedCommand = new AsyncRelayCommand(OnLoadedAsync);
        SumbitCommand = new AsyncRelayCommand(SubmitClickedAsync);
        IncomeCommand = new RelayCommand(IncomeClicked);
        OutcomeCommand = new RelayCommand(OutcomeClicked);
    }

    public override void SetParameter(object parameter)
    {
        if (parameter is not Models.Accounting model)
        {
            return;
        }

        if (model == null)
        {
            IsEidt = false;
            model = new Models.Accounting();
        }
        else
        {
            IsEidt = true;
        }

        GetCategories(model.Type);

        if (model.Type == "收入")
        {
            IsIncome = true;
            IsOutcome = false;
        }
        else
        {
            IsIncome = false;
            IsOutcome = true;
        }

        Accounting = _accountingStorage.GetAccounting(model.Id).Result;
    }

    public Accounting Accounting
    {
        get => _Accounting;
        set => SetProperty(ref _Accounting, value);
    }

    private Accounting _Accounting;

    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private bool _isIncome;

    public bool IsIncome
    {
        get => _isIncome;
        set => SetProperty(ref _isIncome, value);
    }

    private bool _isOutcome;

    public bool IsOutcome
    {
        get => _isOutcome;
        set => SetProperty(ref _isOutcome, value);
    }

    private bool _isEidt;
    public bool IsEidt
    {
        get => _isEidt;
        set => SetProperty(ref _isEidt, value);
    }

    public ICommand OnLoadedCommand { get; }

    public async Task OnLoadedAsync()
    {
        if (!IsEidt)
            Accounting = new Accounting();
    }

    public ICommand SumbitCommand { get; }

    public ICommand IncomeCommand { get; }

    public ICommand OutcomeCommand { get; }

    public async Task SubmitClickedAsync()
    {
        IsLoading = true;
        await _accountingStorage.SaveAccountingAsync(Accounting);
        IsLoading = false;

        await _alertService.AlertAsync("账单提示", "提交账单成功");
        if (!IsEidt)
        {
            Accounting = new Accounting();
        }

        for (int i = 0; i < 19; i++)
        {
            var date = new DateTime(2024, 11, 1);
            var account = new Accounting()
            {
                Type = "支出",
                Category = "餐饮",
                Amount = 10,
                Time = date.AddDays(i),
                Content = "早餐"
            };
            await _accountingStorage.SaveAccountingAsync(account);

            var account1 = new Accounting()
            {
                Type = "支出",
                Category = "购物",
                Amount = 20,
                Time = date.AddDays(i),
                Content = "午餐"
            };
            await _accountingStorage.SaveAccountingAsync(account1);

            var account2 = new Accounting()
            {
                Type = "支出",
                Category = "交通",
                Amount = 25,
                Time = date.AddDays(i),
                Content = "晚餐"
            };
            await _accountingStorage.SaveAccountingAsync(account2);
        }
    }

    public void IncomeClicked()
    {
        this.Accounting.Type = "收入";
        IsIncome = true;
        IsOutcome = false;

        GetCategories(Accounting.Type);
    }

    public void OutcomeClicked()
    {
        this.Accounting.Type = "支出";
        IsIncome = false;
        IsOutcome = true;
        GetCategories(Accounting.Type);
    }

    public ObservableCollection<string> GetCategories(string type)
    {
        CategorySelectList = new ObservableCollection<string>();
        if (type == "收入")
        {
            CategorySelectList.Add("生活费");
            CategorySelectList.Add("生意");
            CategorySelectList.Add("工资");
            CategorySelectList.Add("奖金");
            CategorySelectList.Add("其他人情");
            CategorySelectList.Add("收红包");
            CategorySelectList.Add("退款");
            CategorySelectList.Add("收转账");
            CategorySelectList.Add("其他");
        }
        else
        {
            CategorySelectList.Add("餐饮");
            CategorySelectList.Add("交通");
            CategorySelectList.Add("服饰");
            CategorySelectList.Add("购物");
            CategorySelectList.Add("服务");
            CategorySelectList.Add("教育");
            CategorySelectList.Add("娱乐");
            CategorySelectList.Add("运动");
            CategorySelectList.Add("旅行");
            CategorySelectList.Add("医疗");
        }

        return CategorySelectList;
    }

    public ObservableCollection<string> TypeSelectList { get; set; }

    private ObservableCollection<string> _CategorySelectList;

    public ObservableCollection<string> CategorySelectList
    {
        get => _CategorySelectList;
        set => SetProperty(ref _CategorySelectList, value);
    }
}
