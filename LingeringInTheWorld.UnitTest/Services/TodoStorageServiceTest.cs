using System.Linq.Expressions;
using LingeringInTheWorld.Library.Models;
using LingeringInTheWorld.Library.Services;
using Moq;

namespace LingeringInTheWorld.UnitTest.Services;

 public class TodoStorageServiceTest
    {
        private Mock<IToDoStorage> _mockToDoStorage;
        private TodoStorageService _todoStorageService;

        public TodoStorageServiceTest()
        {
            _mockToDoStorage = new Mock<IToDoStorage>();
            _todoStorageService = new TodoStorageService(_mockToDoStorage.Object);
        }

        [Fact]
        public async Task AddToDoItemAsync_AddsItem()
        {
            // Arrange
            var toDo = new ToDo { Content = "Test Add", Title = "Test Add" };
            _mockToDoStorage.Setup(m => m.AddToDoItemAsync(toDo)).ReturnsAsync(1);

            // Act
            var result = await _todoStorageService.AddToDoItemAsync(toDo);

            // Assert
            Assert.Equal(1, result);
            _mockToDoStorage.Verify(m => m.AddToDoItemAsync(toDo), Times.Once);
        }

        [Fact]
        public async Task DeleteToDoItemAsync_DeletesItem()
        {
            // Arrange
            var id = 1;
            _mockToDoStorage.Setup(m => m.DeleteToDoItemAsync(id)).ReturnsAsync(1);

            // Act
            var result = await _todoStorageService.DeleteToDoItemAsync(id);

            // Assert
            Assert.Equal(1, result);
            _mockToDoStorage.Verify(m => m.DeleteToDoItemAsync(id), Times.Once);
        }

        [Fact]
        public async Task UpdateToDoItemStatusAsync_UpdatesStatus()
        {
            // Arrange
            var id = 1;
            var status = true;
            _mockToDoStorage.Setup(m => m.UpdateToDoItemAsync(id, null, null, null, null, status)).ReturnsAsync(true);

            // Act
            await _todoStorageService.UpdateToDoItemStatusAsync(id, status);

            // Assert
            _mockToDoStorage.Verify(m => m.UpdateToDoItemAsync(id, null, null, null, null, status), Times.Once);
        }

        [Fact]
        public async Task UpdateToDoItemFinishedTimeAsync_UpdatesFinishedTime()
        {
            // Arrange
            var id = 1;
            var finishedTime = DateTime.Now;
            _mockToDoStorage.Setup(m => m.UpdateToDoItemAsync(id, null, finishedTime, null, null, false)).ReturnsAsync(true);

            // Act
            await _todoStorageService.UpdateToDoItemFinishedTimeAsync(id, finishedTime);

            // Assert
            _mockToDoStorage.Verify(m => m.UpdateToDoItemAsync(id, null, finishedTime, null, null, false), Times.Once);
        }

        [Fact]
        public async Task GetAllTodoListAsync_GetsAllItems()
        {
            // Arrange
            var toDos = new List<ToDo>
            {
                new ToDo { Content = "Test 1", Title = "Test 1" },
                new ToDo { Content = "Test 2", Title = "Test 2" }
            };
            _mockToDoStorage.Setup(m => m.GetTodoListAsync(It.IsAny<Expression<Func<ToDo, bool>>>(), 0, int.MaxValue)).ReturnsAsync(toDos);

            // Act
            var result = await _todoStorageService.GetAllTodoListAsync();

            // Assert
            Assert.Equal(toDos.Count, result.Count);
            _mockToDoStorage.Verify(m => m.GetTodoListAsync(It.IsAny<Expression<Func<ToDo, bool>>>(), 0, int.MaxValue), Times.Once);
        }

        [Fact]
        public async Task GetToDoList_GetsFilteredItems()
        {
            // Arrange
            var toDos = new List<ToDo>
            {
                new ToDo { Content = "Test 1", Title = "Test 1" },
                new ToDo { Content = "Test 2", Title = "Test 2" }
            };
            var filter = It.IsAny<Expression<Func<ToDo, bool>>>();
            _mockToDoStorage.Setup(m => m.GetTodoListAsync(filter, 0, 10)).ReturnsAsync(toDos);

            // Act
            var result = await _todoStorageService.GetToDoList(filter, 0, 10);

            // Assert
            Assert.Equal(toDos.Count, result.Count);
            _mockToDoStorage.Verify(m => m.GetTodoListAsync(filter, 0, 10), Times.Once);
        }

        [Fact]
        public async Task GetToDoItem_GetsItem()
        {
            // Arrange
            var id = 1;
            var toDo = new ToDo { Id = id, Content = "Test Get", Title = "Test Get" };
            _mockToDoStorage.Setup(m => m.GetToDoItemAsync(id)).ReturnsAsync(toDo);

            // Act
            var result = await _todoStorageService.GetToDoItem(id);

            // Assert
            Assert.Equal(toDo, result);
            _mockToDoStorage.Verify(m => m.GetToDoItemAsync(id), Times.Once);
        }
    }
