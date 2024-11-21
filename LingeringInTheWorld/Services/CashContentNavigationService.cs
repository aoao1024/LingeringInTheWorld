using System;
using LingeringInTheWorld.Library.Services;
using LingeringInTheWorld.Library.ViewModels;

namespace LingeringInTheWorld.Services;

public class CashContentNavigationService :ICashContentNavigationService
{
    public void NavigateTo(string view, object parameter = null) {
        ViewModelBase viewModel = view switch {
            CashContentNavigationConstant.DetailView => ServiceLocator.Current
                .DetailViewModel,
            _ => throw new Exception("未知的视图。")
        };

        if (parameter != null) {
            viewModel.SetParameter(parameter);
        }

        ServiceLocator.Current.CashMainViewModel.PushContent(viewModel);
    }
}