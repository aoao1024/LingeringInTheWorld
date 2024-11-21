using System;
using LingeringInTheWorld.Library.Services;

namespace LingeringInTheWorld.Services;

public class CashRootNavigationService : ICashRootNavigationService {
    public void NavigateTo(string view) {
        ServiceLocator.Current.CashMainWindowViewModel.Content = view switch {
            CashRootNavigationConstant.InitializationView => ServiceLocator.Current
                .InitializationViewModel,
            CashRootNavigationConstant.MainView => ServiceLocator.Current
                .CashMainViewModel,
            _ => throw new Exception("未知的视图。")
        };
    }
}