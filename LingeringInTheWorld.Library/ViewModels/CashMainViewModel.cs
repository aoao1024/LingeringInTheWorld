using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LingeringInTheWorld.Library.Services;

namespace LingeringInTheWorld.Library.ViewModels;

public class CashMainViewModel : ViewModelBase {
    private readonly IMenuNavigationService _menuNavigationService;

    public CashMainViewModel(IMenuNavigationService menuNavigationService) {
        _menuNavigationService = menuNavigationService;

        OpenPaneCommand = new RelayCommand(OpenPane);
        ClosePaneCommand = new RelayCommand(ClosePane);
        GoBackCommand = new RelayCommand(GoBack);
        OnMenuTappedCommand = new RelayCommand(OnMenuTapped);
    }

    private string _title = "CashBook";

    public string Title {
        get => _title;
        private set => SetProperty(ref _title, value);
    }

    private bool _isPaneOpen;

    public bool IsPaneOpen {
        get => _isPaneOpen;
        private set => SetProperty(ref _isPaneOpen, value);
    }

    private ViewModelBase _content;

    public ViewModelBase Content {
        get => _content;
        private set => SetProperty(ref _content, value);
    }

    public ICommand OpenPaneCommand { get; }

    public void OpenPane() => IsPaneOpen = true;

    public ICommand ClosePaneCommand { get; }

    public void ClosePane() => IsPaneOpen = false;

    public void PushContent(ViewModelBase content) =>
        ContentStack.Insert(0, Content = content);

    public void SetMenuAndContent(string view, ViewModelBase content) {
        ContentStack.Clear();
        PushContent(content);
        SelectedMenuItem =
            CashMenuItem.CashMenuItems.FirstOrDefault(p => p.View == view);
        Title = SelectedMenuItem.Name;
        IsPaneOpen = false;
    }

    private CashMenuItem _selectedMenuItem;

    public CashMenuItem SelectedMenuItem {
        get => _selectedMenuItem;
        set => SetProperty(ref _selectedMenuItem, value);
    }

    public ICommand OnMenuTappedCommand { get; }

    public void OnMenuTapped() {
        if (SelectedMenuItem is null) {
            return;
        }

        _menuNavigationService.NavigateTo(SelectedMenuItem.View);
    }

    public ObservableCollection<ViewModelBase> ContentStack { get; } = [];

    public ICommand GoBackCommand { get; }

    public void GoBack() {
        if (ContentStack.Count <= 1) {
            return;
        }

        ContentStack.RemoveAt(0);
        Content = ContentStack[0];
    }
}

public class CashMenuItem {
    public string View { get; private init; }
    public string Name { get; private init; }

    private CashMenuItem() { }

    private static CashMenuItem TodayView =>
        new() { Name = "本月花销", View = MenuNavigationConstant.MonthView };

    private static CashMenuItem AccountingListView =>
        new() { Name = "账单搜索", View = MenuNavigationConstant.AccountingListView };

    private static CashMenuItem DetailView =>
        new() { Name = "添加账单", View = MenuNavigationConstant.DetailView };

    private static CashMenuItem ExpectedExpensesView =>
       new() { Name = "设置预期花销", View = MenuNavigationConstant.ExpectedExpensesView };

    public static IEnumerable<CashMenuItem> CashMenuItems{ get; } = [
        TodayView, 
        AccountingListView,
        DetailView,
        ExpectedExpensesView
    ];
}