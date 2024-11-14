using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LingeringInTheWorld.Library.Services;

namespace LingeringInTheWorld.Library.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IAppStorage _appStorage;
    
    public MainWindowViewModel(IAppStorage appStorage)
    {
        _appStorage = appStorage;

        OnInitializedCommand = new RelayCommand(OnInitialized);
    }
    
    public void OnInitialized() {
        // if (!_appStorage.IsInitialized) {
        //     _rootNavigationService.NavigateTo(RootNavigationConstant
        //         .InitializationView);
        // } else {
        //     _rootNavigationService.NavigateTo(RootNavigationConstant.MainView);
        //     _menuNavigationService.NavigateTo(MenuNavigationConstant.TodayView);
        // }
    }
    public ICommand OnInitializedCommand { get; }

}