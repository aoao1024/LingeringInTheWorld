using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;
using static SQLite.SQLite3;
using System.Collections.ObjectModel;
using System.Numerics;

namespace LingeringInTheWorld.Library.ViewModels;

public class ExpectedExpensesViewModel : ViewModelBase {
    private IExpectedExpensesStorage _expectedExpensesStorage;

    public ExpectedExpensesViewModel(IExpectedExpensesStorage expectedExpensesStorage) {
        _expectedExpensesStorage = expectedExpensesStorage;

        OnLoadedCommand = new AsyncRelayCommand(OnLoadedAsync);
        SumbitCommand = new AsyncRelayCommand(SubmitClickedAsync);
    }

    private ExpectedExpenses _expectedExpenses;
    public ExpectedExpenses ExpectedExpenses
    {
        get => _expectedExpenses;
        set => SetProperty(ref _expectedExpenses, value);
    }

    private bool _isLoading;

    public bool IsLoading {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand OnLoadedCommand { get; }

    public async Task OnLoadedAsync() {
        _isLoading = true;
        DateTime today = DateTime.Now;
        ExpectedExpenses = await _expectedExpensesStorage.GetMonthExpectedExpensesAsync(today.Year, today.Month);
        if (ExpectedExpenses == null)
        {
            ExpectedExpenses = new ExpectedExpenses()
            {
                Year = today.Year,
                Month = today.Month
            };
        }
    }

    public ICommand SumbitCommand { get; }


    public async Task SubmitClickedAsync()
    {
        IsLoading = true;
        await _expectedExpensesStorage.SaveExpectedExpensesAsync(ExpectedExpenses);
        IsLoading = false;
    }

}
