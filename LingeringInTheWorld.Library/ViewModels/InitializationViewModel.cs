using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LingeringInTheWorld.Library.Services;

namespace LingeringInTheWorld.Library.ViewModels;

public class InitializationViewModel : ViewModelBase {
    private readonly IAppStorage _appStorage;
    private readonly IToDoStorage _toDoStorage;
    private readonly IRootNavigationService _rootNavigationService;
    private readonly IMenuNavigationService _menuNavigationService;

    public InitializationViewModel(IAppStorage appStorage,
        IToDoStorage toDoStorage,
        IRootNavigationService rootNavigationService,
        IMenuNavigationService menuNavigationService) {
        _appStorage = appStorage;
        _toDoStorage = toDoStorage;
        _rootNavigationService = rootNavigationService;
        _menuNavigationService = menuNavigationService;
        OnInitializedCommand = new AsyncRelayCommand(OnInitializedAsync);
    }
    
    private ICommand OnInitializedCommand { get; }

    public async Task OnInitializedAsync() {
        if (!_appStorage.IsInitialized) {
            await _appStorage.InitializeAsync();
        }
        if (!_toDoStorage.IsInitialized)
        {
            await _toDoStorage.InitializeAsync();
        }
        await Task.Delay(1000);
        _rootNavigationService.NavigateTo(RootNavigationConstant.MenuView);
        _menuNavigationService.NavigateTo(MenuNavigationConstant.MainView);
    }
}