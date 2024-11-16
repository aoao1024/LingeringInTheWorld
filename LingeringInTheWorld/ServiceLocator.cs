using System;
using Avalonia;
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
    
    public LedgerViewModel LedgerViewModel =>
        _serviceProvider.GetRequiredService<LedgerViewModel>();
    
    public ToDoListViewModel ToDoListViewModel =>
        _serviceProvider.GetRequiredService<ToDoListViewModel>();
    
    public DiaryDetailViewModel DiaryDetailViewModel =>
        _serviceProvider.GetRequiredService<DiaryDetailViewModel>();
    
    public DiaryAddViewModel DiaryAddViewModel =>
        _serviceProvider.GetRequiredService<DiaryAddViewModel>();
    public ToDoDetailViewModel ToDoDetailViewModel =>
        _serviceProvider.GetRequiredService<ToDoDetailViewModel>();
    
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
        serviceCollection.AddSingleton<ITodayImageService, BingImageService>();
        serviceCollection.AddSingleton<ITodayImageStorage, TodayImageStorage>();
        serviceCollection.AddSingleton<MainWindowViewModel>();
        serviceCollection.AddSingleton<MainViewModel>();
        serviceCollection.AddSingleton<MenuViewModel>();
        serviceCollection.AddSingleton<DiaryViewModel>();
        serviceCollection.AddSingleton<LedgerViewModel>();
        serviceCollection.AddSingleton<ToDoListViewModel>();
        serviceCollection.AddSingleton<DiaryDetailViewModel>();
        serviceCollection.AddSingleton<DiaryAddViewModel>();
        serviceCollection.AddSingleton<ITodoStorageService, TodoStorageService>();
        serviceCollection.AddSingleton<IToDoStorage, ToDoStorage>();
        serviceCollection.AddSingleton<ToDoDetailViewModel>();
        
        //取对象
        _serviceProvider = serviceCollection.BuildServiceProvider();

    }

}