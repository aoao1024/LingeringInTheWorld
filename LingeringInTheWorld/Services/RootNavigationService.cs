using System;
using LingeringInTheWorld.Library.Services;


namespace LingeringInTheWorld.Services;

//导航服务，用于导航到指定的视图，给MainWindowViewModel.Content赋值
public class RootNavigationService : IRootNavigationService {
    public void NavigateTo(string view) {
        ServiceLocator.Current.MainWindowViewModel.Content = view switch {
            RootNavigationConstant.InitializationView => ServiceLocator.Current
                .InitializationViewModel,
            RootNavigationConstant.MenuView => ServiceLocator.Current
                .MenuViewModel,
            _ => throw new Exception("未知的视图。")
        };

    }
}