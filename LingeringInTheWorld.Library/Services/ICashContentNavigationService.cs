namespace LingeringInTheWorld.Library.Services;

public interface ICashContentNavigationService
{
    void NavigateTo(string view, object parameter = null);
}

public static class CashContentNavigationConstant {
    public const string TodayDetailView = nameof(TodayDetailView);

    public const string ResultView = nameof(ResultView);

    public const string DetailView = nameof(DetailView);
}