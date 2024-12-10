using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;
using LingeringInTheWorld.UnitTest.Helpers;
using Moq;

namespace LingeringInTheWorld.UnitTest.Services;

public class ToDoStorageTest :IDisposable
{
    private AppStorage _appStorage;
    private ToDoStorage _toDoStorage;
    public ToDoStorageTest()
    {
        AppStorageHelper.RemoveDatabaseFile();
    }
    public  void Dispose()
    {
        _appStorage.CloseAsync().Wait();
        _toDoStorage.CloseAsync().Wait();
        AppStorageHelper.RemoveDatabaseFile();
    }
    private static IPreferenceStorage GetPreferenceStorage()
        =>new Mock<IPreferenceStorage>().Object;
    [Fact]
    public async Task AddToDoItemAsync_Default()
    {
        _appStorage =await AppStorageHelper.GetInitializedAppStorage();
        _toDoStorage = new ToDoStorage(GetPreferenceStorage());
        ToDo toDo = new ToDo(){Content = "Test Add",Title = "Test Add"};
        int result = await _toDoStorage.AddToDoItemAsync(toDo);
        Assert.True(File.Exists(AppStorage.AppDbPath));
        Assert.True(result > 0);
    }
    [Fact]
    public async Task DeleteToDoItemAsync_Success()
    {
        _appStorage =await AppStorageHelper.GetInitializedAppStorage();
        _toDoStorage = new ToDoStorage(GetPreferenceStorage());
        ToDo toDo1 = new ToDo(){Content = "Test Delete1",Title = "Test Delete1"};
        ToDo toDo2 = new ToDo(){Content = "Test Delete2",Title = "Test Delete2"};
        await _toDoStorage.AddToDoItemAsync(toDo1);
        await _toDoStorage.AddToDoItemAsync(toDo2);
        int result = await _toDoStorage.DeleteToDoItemAsync(toDo1.Id);
        Assert.True(result > 0);
    }
    [Fact]
    public async Task DeleteToDoItemAsync_Fail()
    {
        _appStorage =await AppStorageHelper.GetInitializedAppStorage();
        _toDoStorage = new ToDoStorage(GetPreferenceStorage());
        ToDo toDo1 = new ToDo(){Content = "Test Delete1",Title = "Test Delete1"};
        int result = await _toDoStorage.DeleteToDoItemAsync(toDo1.Id);
        Assert.False(result > 0);
    }
    [Fact]
    public async Task UpdateToDoItemAsync_Deadline_Title_Content_Status_Success()
    {
        _appStorage =await AppStorageHelper.GetInitializedAppStorage();
        _toDoStorage = new ToDoStorage(GetPreferenceStorage());
        ToDo toDo1 = new ToDo(){Content = "Test Add1",Title = "Test Add1",DeadLine = DateTime.Today,Status = false};
        ToDo toDo2 = new ToDo(){Content = "Test Add2",Title = "Test Add2"};
        await _toDoStorage.AddToDoItemAsync(toDo1);
        await _toDoStorage.AddToDoItemAsync(toDo2);
        var deadline = DateTime.Today.AddDays(1);
        var title = "Test Update1";
        var content = "Test Update1";
        var status = true;
        var result = await _toDoStorage.UpdateToDoItemAsync(toDo1.Id, deadline, null, null,null, false);
        var updateToDo1 =await _toDoStorage.TestConnection.FindAsync<ToDo>(toDo1.Id);
        Assert.True(result);
        Assert.True(updateToDo1.DeadLine.Equals(deadline));
        result = await _toDoStorage.UpdateToDoItemAsync(toDo1.Id, null, null,title, null, false);
        updateToDo1=await _toDoStorage.TestConnection.FindAsync<ToDo>(toDo1.Id);
        Assert.True(result);
        Assert.True(updateToDo1.Title.Equals(title));
        result = await _toDoStorage.UpdateToDoItemAsync(toDo1.Id, null, null, null,content, false);
        updateToDo1=await _toDoStorage.TestConnection.FindAsync<ToDo>(toDo1.Id);
        Assert.True(result);
        Assert.True(updateToDo1.Content.Equals(content));
        result = await _toDoStorage.UpdateToDoItemAsync(toDo1.Id, null, null, null,null, status);
        updateToDo1=await _toDoStorage.TestConnection.FindAsync<ToDo>(toDo1.Id);
        Assert.True(result);
        Assert.True(updateToDo1.Status);
    }
    [Fact]
    public async Task UpdateToDoItemAsync_Deadline_Title_Content_Status_Fail()
    {
        _appStorage =await AppStorageHelper.GetInitializedAppStorage();
        _toDoStorage = new ToDoStorage(GetPreferenceStorage());
        ToDo toDo1 = new ToDo(){Content = "Test Add1",Title = "Test Add1",DeadLine = DateTime.Today,Status = false};
        ToDo toDo2 = new ToDo(){Content = "Test Add2",Title = "Test Add2"};
        await _toDoStorage.AddToDoItemAsync(toDo1);
        await _toDoStorage.AddToDoItemAsync(toDo2);
        await _toDoStorage.DeleteToDoItemAsync(toDo1.Id);
        var deadline = DateTime.Today.AddDays(1);
        var title = "Test Update1";
        var content = "Test Update1";
        var status = true;
        var result = await _toDoStorage.UpdateToDoItemAsync(toDo1.Id, deadline, null,null, null, false);
        var updateToDo1 =await _toDoStorage.TestConnection.FindAsync<ToDo>(toDo1.Id);
        Assert.False(result);
        Assert.Null(updateToDo1);
        result = await _toDoStorage.UpdateToDoItemAsync(toDo1.Id, null, null,title, null, false);
        updateToDo1=await _toDoStorage.TestConnection.FindAsync<ToDo>(toDo1.Id);
        Assert.False(result);
        Assert.Null(updateToDo1);
        result = await _toDoStorage.UpdateToDoItemAsync(toDo1.Id, null, null,null, content, false);
        updateToDo1=await _toDoStorage.TestConnection.FindAsync<ToDo>(toDo1.Id);
        Assert.False(result);
        Assert.Null(updateToDo1);
        result = await _toDoStorage.UpdateToDoItemAsync(toDo1.Id, null, null,null, null, status);
        updateToDo1=await _toDoStorage.TestConnection.FindAsync<ToDo>(toDo1.Id);
        Assert.False(result);
        Assert.Null(updateToDo1);
    }


    [Fact]
    public async Task DeleteToDoItemsAsync_Success()
    {
        _appStorage =await AppStorageHelper.GetInitializedAppStorage();
        _toDoStorage = new ToDoStorage(GetPreferenceStorage());
        var toDo1 = new ToDo(){Content = "Test Delete1",Title = "Test Delete1"};
        var toDo2 = new ToDo(){Content = "Test Delete2",Title = "Test Delete2"};
        var toDo3 = new ToDo(){Content = "Test Delete3",Title = "Test Delete3"};
        await _toDoStorage.AddToDoItemAsync(toDo1);
        await _toDoStorage.AddToDoItemAsync(toDo2);
        await _toDoStorage.AddToDoItemAsync(toDo3);
        var ids=new int[]{toDo1.Id,toDo2.Id,toDo3.Id};
        var result =await _toDoStorage.DeleteToDoItemsAsync(ids);
        var todo = await _toDoStorage.TestConnection.FindAsync<ToDo>(toDo3.Id);
        var count = await _toDoStorage.TestConnection.Table<ToDo>().CountAsync();
        Assert.True(result);
        Assert.Equal(0, count);
    }
    [Fact]
    public async Task DeleteToDoItemsAsync_Fail()
    {
        _appStorage =await AppStorageHelper.GetInitializedAppStorage();
        _toDoStorage = new ToDoStorage(GetPreferenceStorage());
        var toDo1 = new ToDo(){Content = "Test Delete1",Title = "Test Delete1"};
        var toDo2 = new ToDo(){Content = "Test Delete2",Title = "Test Delete2"};
        var toDo3 = new ToDo(){Content = "Test Delete3",Title = "Test Delete3"};
        await _toDoStorage.AddToDoItemAsync(toDo1);
        await _toDoStorage.AddToDoItemAsync(toDo2);
        await _toDoStorage.AddToDoItemAsync(toDo3);
        var ids=new int[]{4,5};
        var result =await _toDoStorage.DeleteToDoItemsAsync(ids);
        var count = await _toDoStorage.TestConnection.Table<ToDo>().CountAsync();
        Assert.False(result);
        Assert.Equal(3, count);
    }
}