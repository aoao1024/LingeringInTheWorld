using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LingeringInTheWorld.Library.Services;

namespace LingeringInTheWorld.Library.ViewModels;

public class MenuViewModel: ViewModelBase
{
    private readonly IMenuNavigationService _menuNavigationService;
    
    public MenuViewModel(IMenuNavigationService menuNavigationService) {
        _menuNavigationService = menuNavigationService;
        OpenPaneCommand = new RelayCommand(OpenPane);
        ClosePaneCommand = new RelayCommand(ClosePane);
        GoBackCommand = new RelayCommand(GoBack);
        OnMenuTappedCommand = new RelayCommand(OnMenuTapped);
    }
    
    private string _title = "LingeringInTheWorld";

    public string Title {
        get => _title;
        private set => SetProperty(ref _title, value);
    }
    
    private bool _isPaneOpen;

    public bool IsPaneOpen {
        get => _isPaneOpen;
        private set => SetProperty(ref _isPaneOpen, value);
    }
    
    public ICommand OpenPaneCommand { get; }

    public void OpenPane() => IsPaneOpen = true;

    public ICommand ClosePaneCommand { get; }

    public void ClosePane() => IsPaneOpen = false;
    
    //ObservableCollection是一个动态数据集合，提供了通知功能，当集合发生变化时，会通知绑定的UI元素进行更新
    public ObservableCollection<ViewModelBase> ContentStack { get; } = [];
    
    private ViewModelBase _content;

    public ViewModelBase Content {
        get => _content;
        private set => SetProperty(ref _content, value);
    }
    
    //压栈，将content压入ContentStack
    public void PushContent(ViewModelBase content) =>
        ContentStack.Insert(0, Content = content);
    
    public void SetMenuAndContent(string view, ViewModelBase content) {
        ContentStack.Clear();//清空ContentStack
        PushContent(content);
        SelectedMenuItem =
            MenuItem.MenuItems.FirstOrDefault(p => p.View == view);
        Title = SelectedMenuItem.Name;
        IsPaneOpen = false;
    }
    
    private MenuItem _selectedMenuItem;

    public MenuItem SelectedMenuItem {
        get => _selectedMenuItem;
        set => SetProperty(ref _selectedMenuItem, value);
    }
    
    public ICommand GoBackCommand { get; }
    
    //弹栈，将ContentStack中的第一个元素弹出，并给Content赋值
    public void GoBack() {
        if (ContentStack.Count <= 1) {
            return;
        }

        ContentStack.RemoveAt(0);
        Content = ContentStack[0];
    }
   
    public ICommand OnMenuTappedCommand { get; }

    public void OnMenuTapped() {
        if (SelectedMenuItem is null) {
            return;
        }

        _menuNavigationService.NavigateTo(SelectedMenuItem.View);
    }
    
}

public class MenuItem{
    public string View { get; private init; }
    public string Name { get; private init; }

    private MenuItem() { }

    private static MenuItem MainView =>
        new() { Name = "首页", View = MenuNavigationConstant.MainView };

    private static MenuItem DiaryView =>
        new() { Name = "日记", View = MenuNavigationConstant.DiaryView };

    private static MenuItem LedgerView =>
        new() { Name = "账本", View = MenuNavigationConstant.LedgerView };

    private static MenuItem TodoView =>
        new() { Name = "代办", View = MenuNavigationConstant.ToDoListView };

    public static IEnumerable<MenuItem> MenuItems { get; } = [
        MainView, DiaryView, LedgerView,TodoView
    ];

}