namespace LingeringInTheWorld.Library.Services;

public interface ICashRootNavigationService
{
 
    void NavigateTo(string view);
}

public static class CashRootNavigationConstant {
    public const string InitializationView = nameof(InitializationView);

    public const string MainView = nameof(MainView);
}