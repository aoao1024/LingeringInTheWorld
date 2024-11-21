using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LingeringInTheWorld.Library.Services;

namespace LingeringInTheWorld.Library.ViewModels;

public class CashInitializationViewModel: ViewModelBase {
    private readonly IAccountingStorage _accountStorage;
    private readonly ICashRootNavigationService _rootNavigationService;
    private readonly ICashMenuNavigationService _menuNavigationService;

    public CashInitializationViewModel(IAccountingStorage accountStorage,
        ICashRootNavigationService rootNavigationService,
        ICashMenuNavigationService menuNavigationService) {
        _accountStorage = accountStorage;
        _rootNavigationService = rootNavigationService;
        _menuNavigationService = menuNavigationService;

        OnInitializedCommand = new AsyncRelayCommand(OnInitializedAsync);
    }

    private ICommand OnInitializedCommand { get; }

    public async Task OnInitializedAsync() {
        if (!_accountStorage.IsInitialized) {
            await _accountStorage.InitializeAsync();
        }

        await Task.Delay(1000);

        _rootNavigationService.NavigateTo(CashRootNavigationConstant.MainView);
        _menuNavigationService.NavigateTo(CashMenuNavigationConstant.MonthView);
    }
}