using LingeringInTheWorld.Library.Services;

namespace LingeringInTheWorld.UnitTest.Helpers;

public class ToDoStorageHelper
{
    public static void RemoveDatabaseFile() =>
        File.Delete(ToDoStorage.TodoDbPath);
}