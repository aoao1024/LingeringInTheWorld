using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;
using LingeringInTheWorld.Library.ViewModels;
using Moq;

namespace LingeringInTheWorld.UnitTest.ViewModel;

public class ToDoDetailViewModelTest
{
    [Fact]
    public async Task SubmitAsync_ValidToDoItem() {
        var toDoItem = new ToDo {
            Id = 1,
            Title = "Test Task",
            Content = "This is a test task.",
            Status = false,
            DeadLine = DateTime.Now.AddDays(1)
        };

        var toDoStorageMock = new Mock<IToDoStorage>();
        toDoStorageMock
            .Setup(p => p.UpdateToDoItemAsync(toDoItem.Id, toDoItem.DeadLine, null,toDoItem.Title, toDoItem.Content, toDoItem.Status))
            .Returns(() => Task.FromResult(true));
        var mockToDoStorage = toDoStorageMock.Object;

        var detailViewModel = new ToDoDetailViewModel(mockToDoStorage);
        detailViewModel.ToDo = toDoItem;

        await detailViewModel.SubmitAsync();

        toDoStorageMock.Verify(
            p => p.UpdateToDoItemAsync(toDoItem.Id, toDoItem.DeadLine, null,toDoItem.Title, toDoItem.Content, toDoItem.Status),
            Times.Once);
    }

    [Fact]
    public async Task SubmitAsync_InvalidToDoItem() {
        var toDoItem = new ToDo {
            Id = 1,
            Title = "",
            Content = "",
            Status = false,
            DeadLine = null
        };

        var toDoStorageMock = new Mock<IToDoStorage>();
        toDoStorageMock
            .Setup(p => p.UpdateToDoItemAsync(toDoItem.Id, toDoItem.DeadLine,null, toDoItem.Title, toDoItem.Content, toDoItem.Status))
            .Throws(new Exception("Invalid ToDoItem"));
        var mockToDoStorage = toDoStorageMock.Object;

        var detailViewModel = new ToDoDetailViewModel(mockToDoStorage);
        detailViewModel.ToDo = toDoItem;

        await Assert.ThrowsAsync<Exception>(() => detailViewModel.SubmitAsync());

        toDoStorageMock.Verify(
            p => p.UpdateToDoItemAsync(toDoItem.Id, toDoItem.DeadLine, null,toDoItem.Title, toDoItem.Content, toDoItem.Status),
            Times.Once);
    }
}