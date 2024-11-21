using System.Linq.Expressions;
using AvaloniaInfiniteScrolling;
using CommunityToolkit.Mvvm.Input;
using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Collections.Generic;

namespace LingeringInTheWorld.Library.ViewModels;

public class AccountingListViewModel : ViewModelBase {
    private readonly IAccountingStorage _accountingStorage;

    private readonly IContentNavigationService _contentNavigationService;

    private Expression<Func<Accounting, bool>> _where;

    public AccountingListViewModel(IAccountingStorage accountingStorage, IContentNavigationService contentNavigationService) {
        _contentNavigationService = contentNavigationService;
        _accountingStorage = accountingStorage;

        OnLoadedCommand = new AsyncRelayCommand(OnLoadedAsync);
        ShowAccountingCommand = new RelayCommand<Accounting>(ShowAccounting);
        SumbitCommand = new AsyncRelayCommand(SubmitClickedAsync);

        TypeSelectList = new ObservableCollection<string>();
        TypeSelectList.Add("所有");
        TypeSelectList.Add("收入");
        TypeSelectList.Add("支出");

        Month = null;//new DateTimeOffset(DateTime.Now);
        SelectedType = "所有";

        AccountingCollection = new ObservableCollection<Accounting>();
    }

    private bool _canLoadMore;

    public async Task OnLoadedAsync()
    {
        _canLoadMore = true;
        if (AccountingCollection != null)
            AccountingCollection.Clear();

        AccountingCollection = await _accountingStorage.GetAccountingAsync(Month, SelectedType);
    }

    private IList<Accounting> _accountingCollection;
    public IList<Accounting> AccountingCollection {
        get => _accountingCollection;
        private set => SetProperty(ref _accountingCollection, value);
    }


    public ObservableCollection<string> TypeSelectList { get; set; }



    private string _status;

    public string Status {
        get => _status;
        private set => SetProperty(ref _status, value);
    }

    private string _selectedType;

    public string SelectedType
    {
        get => _selectedType;
        private set => SetProperty(ref _selectedType, value);
    }

    private DateTimeOffset? _month;

    public DateTimeOffset? Month
    {
        get => _month;
        private set => SetProperty(ref _month, value);
    }

    public const string Loading = "正在载入";

    public const string NoResult = "没有满足条件的结果";

    public const string NoMoreResult = "没有更多结果";

    public const int PageSize = 20;

    public IRelayCommand<Accounting> ShowAccountingCommand { get; }

    public void ShowAccounting(Accounting Accounting) =>
        _contentNavigationService.NavigateTo(ContentNavigationConstant.DetailView, Accounting);

    public ICommand SumbitCommand { get; }
    public ICommand OnLoadedCommand { get; }

    public async Task SubmitClickedAsync()
    {
        AccountingCollection = await _accountingStorage.GetAccountingAsync(Month, SelectedType);
    }
}