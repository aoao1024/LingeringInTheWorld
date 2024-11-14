using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LingeringInTheWorld.Library.Services;

namespace LingeringInTheWorld.Library.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IAppStorage _appStorage;
    private readonly IRootNavigationService _rootNavigationService;
    private readonly IMenuNavigationService _menuNavigationService;

    public MainWindowViewModel(IAppStorage appStorage,
        IRootNavigationService rootNavigationService,
        IMenuNavigationService menuNavigationService)
    {
        _appStorage = appStorage;
        _rootNavigationService = rootNavigationService;
        _menuNavigationService = menuNavigationService;

        OnInitializedCommand = new RelayCommand(OnInitialized);
    }
    
    private ViewModelBase _content;
    
    public ViewModelBase Content {
        get => _content;
        set => SetProperty(ref _content, value);
    }
    
    public void OnInitialized() {
        // if (!_appStorage.IsInitialized) {
        //     _rootNavigationService.NavigateTo(RootNavigationConstant
        //         .InitializationView);
        // } else {
        //     _rootNavigationService.NavigateTo(RootNavigationConstant.MenuView);
        //     _menuNavigationService.NavigateTo(MenuNavigationConstant.MainView);
        // }
        
        _rootNavigationService.NavigateTo(RootNavigationConstant.MenuView);
        _menuNavigationService.NavigateTo(MenuNavigationConstant.MainView);
    }
    public ICommand OnInitializedCommand { get; }

}