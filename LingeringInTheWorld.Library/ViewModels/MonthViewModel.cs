using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;
using CommunityToolkit.Mvvm.Input;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LingeringInTheWorld.Library.ViewModels
{
    public class MonthViewModel : ViewModelBase
    {
        private IExpectedExpensesStorage _expectedExpensesStorage;
        private IMonthStatisticsService _monthStatisticsService;
        private IAlertService _alertService;

        public MonthViewModel(IExpectedExpensesStorage expectedExpensesStorage, 
            IMonthStatisticsService monthStatisticsService,
            IAlertService alertService)
        {
            _expectedExpensesStorage = expectedExpensesStorage;
            _monthStatisticsService = monthStatisticsService;
            _alertService = alertService;

            OnLoadedCommand = new AsyncRelayCommand(OnLoadedAsync);
            _monthStatisticsService = monthStatisticsService;
        }

        private MonthStatistics _monthStatistics;
        public MonthStatistics MonthStatistics
        {
            get => _monthStatistics;
            set => SetProperty(ref _monthStatistics, value);
        }

        private ExpectedExpenses _expectedExpenses;
        public ExpectedExpenses ExpectedExpenses
        {
            get => _expectedExpenses;
            set => SetProperty(ref _expectedExpenses, value);
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand OnLoadedCommand { get; }

        public async Task OnLoadedAsync()
        {
            IsLoading = true;

            DateTime today = DateTime.Now;
            MonthStatistics = await _monthStatisticsService.GetMonthStatisticsAsync(today);
            ExpectedExpenses = await _expectedExpensesStorage.GetMonthExpectedExpensesAsync(today.Year, today.Month);

            IsLoading = false;

            if (ExpectedExpenses != null && MonthStatistics.TotalOutcome > ExpectedExpenses.Value)
            {
                await _alertService.AlertAsync("警告", "您本月总体花销已预期花销，请开源节流！");
            }
        }
    }
}
