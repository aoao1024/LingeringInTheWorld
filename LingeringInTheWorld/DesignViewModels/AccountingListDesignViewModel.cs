using AvaloniaInfiniteScrolling;
using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;
using LingeringInTheWorld.Library.ViewModels;
using System;

namespace CashBook.DesignViewModels;

public class AccountingListDesignViewModel : AccountingListViewModel {
    public AccountingListDesignViewModel(IAccountingStorage accountingStorage,
        IContentNavigationService contentNavigationService) :
        base(accountingStorage, contentNavigationService) { }

    public new AvaloniaInfiniteScrollCollection<Accounting> AccountingCollection { get; } = [
        new Accounting
        {
            
            Type="֧��",
            Category = "����",
            Time = DateTime.Now,
            Amount = 12,
            Content = "�в�"
        },
    ];

    public new string Status { get; } = "Loading...";
}