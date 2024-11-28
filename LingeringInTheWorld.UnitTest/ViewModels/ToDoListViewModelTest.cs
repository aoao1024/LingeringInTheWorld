using System.Collections;
using System.Linq.Expressions;
using AvaloniaInfiniteScrolling;
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
        var deleteFailTest = toDoListViewModel.ToDoCollection[0];
        await toDoListViewModel.DeleteToDoItemAsync(toDoListViewModel.ToDoCollection[0]);
        var toDo = await _toDoStorage.TestConnection.FindAsync<ToDo>(toDoList[0].Id);
        Assert.Null(toDo);
        Assert.DoesNotContain(deleteFailTest,toDoListViewModel.ToDoCollection);
        Assert.Equal(toDoList[1].Id,toDoListViewModel.ToDoCollection[0].ToDo.Id);
        Assert.Equal(toDoList[2].Id,toDoListViewModel.ToDoCollection[1].ToDo.Id);
    }
    [Fact]
    public async Task DeleteToDoItemAsync_Fail()
    {
        await PrepareTestData();
        TodoStorageService todoStorageService = new TodoStorageService(_toDoStorage);
        var toDoListViewModel = new ToDoListViewModel(todoStorageService,null);
        await toDoListViewModel.OnInitializeAsync();
        Assert.NotEmpty(toDoListViewModel.ToDoCollection);
        var deleteFailTest = toDoListViewModel.ToDoCollection[0];
        await toDoListViewModel.DeleteToDoItemAsync(toDoListViewModel.ToDoCollection[0]);
        int result=await toDoListViewModel.DeleteToDoItemAsync(deleteFailTest);
        Assert.Equal(0,result);
    }

    [Fact]
    public async Task OnloadMoreItemsAsync_Success()
    {
        var todoStorageService = new Mock<ITodoStorageService>();
        var toDoListViewModel = new ToDoListViewModel(todoStorageService.Object,null);
        var todo1 = new ToDo { Title = "test1", Content = "test1" };
        var todo2 = new ToDo { Title = "test2", Content = "test2" };
        var todo3 = new ToDo { Title = "test3", Content = "test3" };
        var toDoItems=new List<ToDo> { todo1,todo2,todo3 };
        var toDoItemViewModels= new List<ToDoItemViewModel>
        {
            new(todo1, toDoListViewModel),
            new(todo2, toDoListViewModel),
            new(todo3, toDoListViewModel)
        };
        todoStorageService.Setup(t => t.GetToDoList(It.IsAny<Expression<Func<ToDo, bool>>>(), 0, 10))
            .ReturnsAsync(toDoItems);
        var loadMoreResult =await toDoListViewModel.ToDoCollection.OnLoadMore();
        var loadMoreResultList = loadMoreResult.ToList();
        Assert.NotEmpty(loadMoreResultList);
        Assert.Equal(toDoItemViewModels[0].ToDo,loadMoreResultList[0].ToDo);
        Assert.Equal(toDoItemViewModels[1].ToDo,loadMoreResultList[1].ToDo);
        Assert.Equal(toDoItemViewModels[2].ToDo,loadMoreResultList[2].ToDo);
    }

    [Fact]
    public void AddViewNavigation()
    {
        var contentNavigationService=new Mock<IContentNavigationService>();
        var toDoListViewModel= new ToDoListViewModel(null, contentNavigationService.Object);
        toDoListViewModel.AddToDo();
        contentNavigationService.Verify(c=>c.NavigateTo(ContentNavigationConstant.NewToDoItemView,null),Times.Once);
    }
    [Fact]
    public void DetailViewNavigation()
    {
        var todo1 = new ToDo { Title = "test1", Content = "test1" };
        var contentNavigationService=new Mock<IContentNavigationService>();
        var toDoListViewModel= new ToDoListViewModel(null, contentNavigationService.Object);
        var toDoItemViewModel = new ToDoItemViewModel(todo1, toDoListViewModel);
        toDoListViewModel.ShowToDoItemDetailInfo(toDoItemViewModel);
        contentNavigationService.Verify(c=>c.NavigateTo(ContentNavigationConstant.ToDoDetailView,toDoItemViewModel.ToDo),Times.Once);
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
        if (_appStorage != null)
        {
            _appStorage.CloseAsync().Wait();
        }
        if (_toDoStorage!=null)
        {
            _toDoStorage.CloseAsync().Wait();
        }
        AppStorageHelper.RemoveDatabaseFile();
    }
}