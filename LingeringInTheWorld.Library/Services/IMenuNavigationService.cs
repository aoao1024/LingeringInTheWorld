namespace LingeringInTheWorld.Library.Services;

public interface IMenuNavigationService
{
    void NavigateTo(string view);
}

public static class MenuNavigationConstant {
    public const string MainView = nameof(MainView);

    public const string DiaryView = nameof(DiaryView);

    public const string LedgerView = nameof(LedgerView);
    
    public const string TodoView = nameof(TodoView);
}