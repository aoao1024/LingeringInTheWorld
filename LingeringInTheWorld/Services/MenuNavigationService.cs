using System;
using LingeringInTheWorld.Library.Services;
using LingeringInTheWorld.Library.ViewModels;

namespace LingeringInTheWorld.Services;

public class MenuNavigationService: IMenuNavigationService {
    public void NavigateTo(string view) {
        ViewModelBase viewModel = view switch {
            MenuNavigationConstant.MainView => ServiceLocator.Current
                .MainViewModel,
            MenuNavigationConstant.DiaryView => ServiceLocator.Current
                .DiaryViewModel,
            MenuNavigationConstant.LedgerView => ServiceLocator.Current
                .LedgerViewModel, 
            MenuNavigationConstant.TodoView => ServiceLocator.Current
                .TodoViewModel,
            _ => throw new Exception("未知的视图。")
        };

        ServiceLocator.Current.MenuViewModel.SetMenuAndContent(view, viewModel);
    }
}