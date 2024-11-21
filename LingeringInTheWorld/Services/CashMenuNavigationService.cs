using System;
using LingeringInTheWorld.Library.Services;
using LingeringInTheWorld.Library.ViewModels;

namespace LingeringInTheWorld.Services;

public class CashMenuNavigationService : ICashMenuNavigationService 
{
    public void NavigateTo(string view, object parameter = null) {
        ViewModelBase viewModel = view switch {
            CashMenuNavigationConstant.MonthView => ServiceLocator.Current
                .MonthViewModel,
            CashMenuNavigationConstant.AccountingListView => ServiceLocator.Current
                .AccountingListViewModel,
            CashMenuNavigationConstant.DetailView => ServiceLocator.Current
                .DetailViewModel,
            CashMenuNavigationConstant.ExpectedExpensesView => ServiceLocator.Current
                .ExpectedExpensesViewModel,
            _ => throw new Exception("未知的视图。")
        };

        if (parameter is not null) {
            viewModel.SetParameter(parameter);
        }

        ServiceLocator.Current.CashMainViewModel.SetMenuAndContent(view, viewModel);
    }
}