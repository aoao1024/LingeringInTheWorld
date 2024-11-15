using System;
using LingeringInTheWorld.Library.Services;
using LingeringInTheWorld.Library.ViewModels;

namespace LingeringInTheWorld.Services;

public class ContentNavigationService : IContentNavigationService {
    public void NavigateTo(string view, object parameter = null) {
        ViewModelBase viewModel = view switch {
            ContentNavigationConstant.DiaryDetailView => ServiceLocator.Current
                .DiaryDetailViewModel,
            ContentNavigationConstant.DiaryAddView => ServiceLocator.Current
                .DiaryAddViewModel,
            ContentNavigationConstant.ToDoDetailView => ServiceLocator.Current
                .ToDoDetailViewModel,
            _ => throw new Exception("未知的视图。")
        };
        
        viewModel.SetParameter(parameter);
        
        ServiceLocator.Current.MenuViewModel.PushContent(viewModel);
    }
}