namespace LingeringInTheWorld.Library.Services;

public interface IRootNavigationService {
    void NavigateTo(string view);
}

public static class RootNavigationConstant {
    public const string InitializationView = nameof(InitializationView);

    public const string MenuView = nameof(MenuView);
}