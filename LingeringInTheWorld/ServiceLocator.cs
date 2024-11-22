using System;
using Avalonia;
using CashBook.DesignViewModels;
using LingeringInTheWorld.Library.Services;
using LingeringInTheWorld.Library.ViewModels;
using LingeringInTheWorld.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LingeringInTheWorld;

//服务定位器
public class ServiceLocator
{
    private readonly IServiceProvider _serviceProvider;

    private static ServiceLocator _current;
    
    //从当前应用程序资源中获取ServiceLocator实例
    public static ServiceLocator Current {
        get
        {
            if (_current is not null) {
                return _current;
            }

            if (Application.Current.TryGetResource(nameof(ServiceLocator),
                    null, out var resource) &&
                resource is ServiceLocator serviceLocator) {
                return _current = serviceLocator;
            }

            throw new Exception("理论上来讲不应该发生这种情况。");
        }
    }
    
    public InitializationViewModel InitializationViewModel => 
        _serviceProvider.GetRequiredService<InitializationViewModel>();
    public CashInitializationViewModel CashInitializationViewModel => 
        _serviceProvider.GetRequiredService<CashInitializationViewModel>();
    
    public MainWindowViewModel MainWindowViewModel => 
        _serviceProvider.GetRequiredService<MainWindowViewModel>();
    
    public MainViewModel MainViewModel =>
        _serviceProvider.GetRequiredService<MainViewModel>();

    // TODO Delete this
    public IRootNavigationService RootNavigationService =>
        _serviceProvider.GetRequiredService<IRootNavigationService>();

    public MenuViewModel MenuViewModel =>
        _serviceProvider.GetRequiredService<MenuViewModel>();
    
    public DiaryViewModel DiaryViewModel =>
        _serviceProvider.GetRequiredService<DiaryViewModel>();
    
    public ToDoListViewModel ToDoListViewModel =>
        _serviceProvider.GetRequiredService<ToDoListViewModel>();
    
    public DiaryDetailViewModel DiaryDetailViewModel =>
        _serviceProvider.GetRequiredService<DiaryDetailViewModel>();
    
    public DiaryAddViewModel DiaryAddViewModel =>
        _serviceProvider.GetRequiredService<DiaryAddViewModel>();
    public ToDoDetailViewModel ToDoDetailViewModel =>
        _serviceProvider.GetRequiredService<ToDoDetailViewModel>();

    public NewToDoItemViewModel NewToDoItemViewModel =>
        _serviceProvider.GetRequiredService<NewToDoItemViewModel>();
    
    public CashMainWindowViewModel CashMainWindowViewModel =>
        _serviceProvider.GetService<CashMainWindowViewModel>();

    public CashMainViewModel CashMainViewModel =>
        _serviceProvider.GetService<CashMainViewModel>();

    public AccountingListDesignViewModel AccountingListDesignViewModel =>
        _serviceProvider.GetService<AccountingListDesignViewModel>();


    public AccountingListViewModel AccountingListViewModel =>
        _serviceProvider.GetService<AccountingListViewModel>();

    public AccountingListDesignViewModel ResultDesignViewModel =>
        _serviceProvider.GetService<AccountingListDesignViewModel>();

    public DetailViewModel DetailViewModel =>
        _serviceProvider.GetService<DetailViewModel>();
    
    public DetailViewModel1 DetailViewModel1 =>
        _serviceProvider.GetService<DetailViewModel1>();

    public MonthViewModel MonthViewModel =>
        _serviceProvider.GetService<MonthViewModel>();

    public ExpectedExpensesViewModel ExpectedExpensesViewModel =>
        _serviceProvider.GetRequiredService<ExpectedExpensesViewModel>();
    
    public ServiceLocator()
    {
        //注册对象
        var serviceCollection = new ServiceCollection();
        
        serviceCollection.AddSingleton<IPreferenceStorage,FilePreferenceStorage>();
        serviceCollection.AddSingleton<IAppStorage, AppStorage>();
        serviceCollection.AddSingleton<IAlertService, AlertService>();
        serviceCollection.AddSingleton<IWeatherService, HeFengWeatherService>();
        serviceCollection.AddSingleton<IRootNavigationService, RootNavigationService>();
        serviceCollection.AddSingleton<IMenuNavigationService, MenuNavigationService>();
        serviceCollection.AddSingleton<IContentNavigationService, ContentNavigationService>();
        serviceCollection.AddSingleton<InitializationViewModel>();
        serviceCollection.AddSingleton<CashInitializationViewModel>();
        serviceCollection.AddSingleton<ITodayImageService, BingImageService>();
        serviceCollection.AddSingleton<ITodayImageStorage, TodayImageStorage>();
        serviceCollection.AddSingleton<MainWindowViewModel>();
        serviceCollection.AddSingleton<MainViewModel>();
        serviceCollection.AddSingleton<MenuViewModel>();
        serviceCollection.AddSingleton<DiaryViewModel>();
        serviceCollection.AddSingleton<ToDoListViewModel>();
        serviceCollection.AddSingleton<DiaryDetailViewModel>();
        serviceCollection.AddSingleton<DiaryAddViewModel>();
        serviceCollection.AddSingleton<ITodoStorageService, TodoStorageService>();
        serviceCollection.AddSingleton<IToDoStorage, ToDoStorage>();
        serviceCollection.AddSingleton<ToDoDetailViewModel>();
        serviceCollection.AddSingleton<ILocationService, IpInfoLocationService>();
        serviceCollection.AddSingleton<NewToDoItemViewModel>();
        serviceCollection.AddSingleton<CashMainWindowViewModel>();
        serviceCollection.AddSingleton<CashMainViewModel>();
        serviceCollection.AddSingleton<AccountingListViewModel>();
        serviceCollection.AddSingleton<DetailViewModel>();
        serviceCollection.AddSingleton<DetailViewModel1>();
        serviceCollection.AddSingleton<MonthViewModel>();
        serviceCollection.AddSingleton<ExpectedExpensesViewModel>();

        serviceCollection
            .AddSingleton<ICashRootNavigationService, CashRootNavigationService>();
        serviceCollection
            .AddSingleton<ICashMenuNavigationService, CashMenuNavigationService>();
        serviceCollection
            .AddSingleton<ICashContentNavigationService,
                CashContentNavigationService>();
        serviceCollection.AddSingleton<IAccountingStorage, AccountingStorage>();
        serviceCollection.AddSingleton<IExpectedExpensesStorage, ExpectedExpensesStorage>();
        serviceCollection.AddSingleton<IMonthStatisticsService, MonthStatisticsService>();

        serviceCollection
            .AddSingleton<IPreferenceStorage, FilePreferenceStorage>();
        serviceCollection.AddSingleton<IAlertService, AlertService>();

        serviceCollection.AddSingleton<AccountingListDesignViewModel>();
        
        
        //取对象
        _serviceProvider = serviceCollection.BuildServiceProvider();

    }

}