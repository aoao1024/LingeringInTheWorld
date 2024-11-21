namespace LingeringInTheWorld.Library.Services;

public interface IContentNavigationService
{
    void NavigateTo(string view, object parameter = null);  //导航目标，可选参数
}

public static class ContentNavigationConstant
{
    public const string DiaryView = nameof(DiaryView);
    public const string DiaryDetailView = nameof(DiaryDetailView);
    public const string DiaryAddView = nameof(DiaryAddView);
    public const string ToDoDetailView = nameof(ToDoDetailView);
    public const string NewToDoItemView = nameof(NewToDoItemView);
}