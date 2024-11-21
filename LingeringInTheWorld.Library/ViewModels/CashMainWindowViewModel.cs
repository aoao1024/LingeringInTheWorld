using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LingeringInTheWorld.Library.Services;

namespace LingeringInTheWorld.Library.ViewModels;

public class CashMainWindowViewModel : ViewModelBase {
    private readonly IAccountingStorage _accountStorage;
    private readonly ICashRootNavigationService _rootNavigationService;
    private readonly ICashMenuNavigationService _menuNavigationService;

    public CashMainWindowViewModel(IAccountingStorage accountStorage,
        ICashRootNavigationService rootNavigationService,
        ICashMenuNavigationService menuNavigationService) {
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
            _rootNavigationService.NavigateTo(CashRootNavigationConstant
                .InitializationView);
        } else {
            _rootNavigationService.NavigateTo(CashRootNavigationConstant.MainView);
            _menuNavigationService.NavigateTo(CashMenuNavigationConstant.MonthView);
        }
    }
}