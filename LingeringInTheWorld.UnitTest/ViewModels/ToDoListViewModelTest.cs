using System.Collections;
using DynamicData;
using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;
using LingeringInTheWorld.Library.ViewModels;
using LingeringInTheWorld.UnitTest.Helpers;
using Moq;

namespace LingeringInTheWorld.UnitTest.ViewModels;

public class ToDoListViewModelTest :IDisposable
{
    private AppStorage _appStorage;
    private ToDoStorage _toDoStorage;
    private ToDo[] toDoList;
    public ToDoListViewModelTest()
    {
        AppStorageHelper.RemoveDatabaseFile();
        
    }

    [Fact]
    public async Task SetToDoItemFinishStatusAsync_Default()
    {
        await PrepareTestData();
        TodoStorageService todoStorageService = new TodoStorageService(_toDoStorage);
        var toDoListViewModel = new ToDoListViewModel(todoStorageService,null);
        await toDoListViewModel.OnInitializeAsync();
        Assert.NotEmpty(toDoListViewModel.ToDoCollection);
        await toDoListViewModel.SetToDoItemFinishStatusAsync(toDoListViewModel.ToDoCollection[0]);
        var toDo = await _toDoStorage.TestConnection.FindAsync<ToDo>(toDoList[0].Id);
        Assert.True(toDoList[1].Id==toDoListViewModel.ToDoCollection[0].ToDo.Id);
        Assert.True(toDoList[0].Id==toDoListViewModel.ToDoCollection[2].ToDo.Id);
        Assert.True(toDo.Status);
        Assert.True(toDoListViewModel.ToDoCollection[2].ToDo.Status);
    }
    [Fact]
    public async Task DeleteToDoItemAsync_Success()
    {
        await PrepareTestData();
        TodoStorageService todoStorageService = new TodoStorageService(_toDoStorage);
        var toDoListViewModel = new ToDoListViewModel(todoStorageService,null);
        await toDoListViewModel.OnInitializeAsync();
        Assert.NotEmpty(toDoListViewModel.ToDoCollection);
        await toDoListViewModel.DeleteToDoItemAsync(toDoListViewModel.ToDoCollection[0]);
        
    }
    private static IPreferenceStorage GetPreferenceStorage()
        =>new Mock<IPreferenceStorage>().Object;

    private async Task PrepareTestData()
    {
        _appStorage=await AppStorageHelper.GetInitializedAppStorage();
        _toDoStorage = new ToDoStorage(GetPreferenceStorage());
        var toDo1 = new ToDo() { Content = "Test Delete1", Title = "Test Delete1" ,Status = false};
        var toDo2 = new ToDo(){Content = "Test Delete2",Title = "Test Delete2",Status = true};
        var toDo3 = new ToDo(){Content = "Test Delete3",Title = "Test Delete3",Status = false};
        await _toDoStorage.AddToDoItemAsync(toDo1);
        await _toDoStorage.AddToDoItemAsync(toDo2);
        await _toDoStorage.AddToDoItemAsync(toDo3);
        toDoList = new[] { toDo1, toDo2, toDo3 };
    }
    public void Dispose()
    {
        _appStorage.CloseAsync().Wait();
        _toDoStorage.CloseAsync().Wait();
        AppStorageHelper.RemoveDatabaseFile();
    }
}