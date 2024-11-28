using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;
using LingeringInTheWorld.Library.ViewModels;
using Moq;

namespace LingeringInTheWorld.UnitTest.ViewModel;

public class NewToDoItemViewModelTest
{
    [Fact]
    public async Task AddNewToDoItemATask_ValidToDoItem() {
        var toDoItem = new ToDo {
            Title = "Test Task",
            Content = "This is a test task.",
            Status = false,
            DeadLine = DateTime.Now.AddDays(1)
        };

        var todoStorageServiceMock = new Mock<ITodoStorageService>();
        todoStorageServiceMock
            .Setup(p => p.AddToDoItemAsync(toDoItem))
            .Returns(() => Task.FromResult(1)); // 返回插入的行数
        var mockTodoStorageService = todoStorageServiceMock.Object;

        var menuNavigationServiceMock = new Mock<IMenuNavigationService>();
        var mockMenuNavigationService = menuNavigationServiceMock.Object;

        var newToDoItemViewModel = new NewToDoItemViewModel(mockTodoStorageService, mockMenuNavigationService);
        newToDoItemViewModel.ToDo = toDoItem;

        await newToDoItemViewModel.AddNewToDoItemATask();

        todoStorageServiceMock.Verify(
            p => p.AddToDoItemAsync(toDoItem),
            Times.Once);
        menuNavigationServiceMock.Verify(
            p => p.NavigateTo(MenuNavigationConstant.ToDoListView),
            Times.Once);
    }

    [Fact]
    public async Task AddNewToDoItemATask_InvalidToDoItem() {
        var toDoItem = new ToDo {
            Title = "",
            Content = "",
            Status = false,
            DeadLine = null
        };

        var todoStorageServiceMock = new Mock<ITodoStorageService>();
        todoStorageServiceMock
            .Setup(p => p.AddToDoItemAsync(toDoItem))
            .Throws(new Exception("Invalid ToDoItem"));
        var mockTodoStorageService = todoStorageServiceMock.Object;

        var menuNavigationServiceMock = new Mock<IMenuNavigationService>();
        var mockMenuNavigationService = menuNavigationServiceMock.Object;

        var newToDoItemViewModel = new NewToDoItemViewModel(mockTodoStorageService, mockMenuNavigationService);
        newToDoItemViewModel.ToDo = toDoItem;

        await Assert.ThrowsAsync<Exception>(() => newToDoItemViewModel.AddNewToDoItemATask());

        todoStorageServiceMock.Verify(
            p => p.AddToDoItemAsync(toDoItem),
            Times.Once);
        menuNavigationServiceMock.Verify(
            p => p.NavigateTo(MenuNavigationConstant.ToDoListView),
            Times.Never);
    }
}