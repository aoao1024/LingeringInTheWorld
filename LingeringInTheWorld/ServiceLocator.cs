using System;
using LingeringInTheWorld.Library.Services;
using LingeringInTheWorld.Library.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace LingeringInTheWorld;

//服务定位器
public class ServiceLocator
{
    private readonly IServiceProvider _serviceProvider;

    public MainWindowViewModel MainWindowViewModel => 
        _serviceProvider.GetRequiredService<MainWindowViewModel>();

    
    public ServiceLocator()
    {
        //注册对象
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<MainWindowViewModel>();
        serviceCollection.AddSingleton<IAppStorage, AppStorage>();
        
        //取对象
        _serviceProvider = serviceCollection.BuildServiceProvider();

    }

}