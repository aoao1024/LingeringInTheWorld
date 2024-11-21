using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LingeringInTheWorld.Library.Services;

namespace LingeringInTheWorld.Library.ViewModels;

public class CashMainWindowViewModel : ViewModelBase {
    private readonly IAccountingStorage _accountStorage;
    private readonly IRootNavigationService _rootNavigationService;
    private readonly IMenuNavigationService _menuNavigationService;

    public CashMainWindowViewModel(IAccountingStorage accountStorage,
        IRootNavigationService rootNavigationService,
        IMenuNavigationService menuNavigationService) {
        _accountStorage = accountStorage;
        _rootNavigationService = rootNavigationService;
        _menuNavigationService = menuNavigationService;

        OnInitializedCommand = new RelayCommand(OnInitialized);
    }

    private ViewModelBase _content;

    public ViewModelBase Content {
        get => _content;
        set => SetProperty(ref _content, value);
    }

    public ICommand OnInitializedCommand { get; }

    public void OnInitialized() {
        if (!_accountStorage.IsInitialized) {
            _rootNavigationService.NavigateTo(RootNavigationConstant
                .InitializationView);
        } else {
            _rootNavigationService.NavigateTo(RootNavigationConstant.MenuView);
            _menuNavigationService.NavigateTo(MenuNavigationConstant.MainView);
        }
    }
}