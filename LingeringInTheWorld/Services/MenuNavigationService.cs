using System;
using LingeringInTheWorld.Library.Services;
using LingeringInTheWorld.Library.ViewModels;

namespace LingeringInTheWorld.Services;

public class MenuNavigationService: IMenuNavigationService {
    public void NavigateTo(string view, object parameter = null) {
        ViewModelBase viewModel = view switch {
            MenuNavigationConstant.MainView => ServiceLocator.Current
                .MainViewModel,
            MenuNavigationConstant.DiaryView => ServiceLocator.Current
                .DiaryViewModel,
            MenuNavigationConstant.LedgerView => ServiceLocator.Current
                .LedgerViewModel, 
            MenuNavigationConstant.MonthView => ServiceLocator.Current
                .MonthViewModel,
            MenuNavigationConstant.AccountingListView => ServiceLocator.Current
                .AccountingListViewModel,
            MenuNavigationConstant.DetailView => ServiceLocator.Current
                .DetailViewModel,
            MenuNavigationConstant.ToDoListView => ServiceLocator.Current
                .ToDoListViewModel,
            _ => throw new Exception("未知的视图。")
        };
        if (parameter is not null) {
            viewModel.SetParameter(parameter);
        }

        ServiceLocator.Current.MenuViewModel.SetMenuAndContent(view, viewModel);
    }
}