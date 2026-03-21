using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;
using TodoApi.Models;
using TodoApi.Services;
using Microsoft.EntityFrameworkCore;

namespace TodoApi.Tests;

public class TodoServiceTests
{
    [Fact]
    public void AddTodoItem_ShouldAddItemToDatabase()
    {
        // Arrange
        var mockDbSet = new Mock<DbSet<TodoItem>>();
        var mockContext = new Mock<AppDbContext>();
        mockContext.Setup(c => c.TodoItems).Returns(mockDbSet.Object);
        
        var todoService = new TodoService(mockContext.Object);
        var newTodoItem = new TodoItem { Id = Guid.NewGuid(), Title = "Test Task", IsCompleted = false };

        // Act
        todoService.AddTodoItem(newTodoItem);

        // Assert
        mockDbSet.Verify(m => m.Add(It.IsAny<TodoItem>()), Times.Once());
        mockContext.Verify(m => m.SaveChanges(), Times.Once());
    }

    [Fact]
    public void GetAllTodoItems_ShouldReturnAllItems()
    {
        // Arrange
        var todoItems = new List<TodoItem>
        {
            new TodoItem { Id = Guid.NewGuid(), Title = "Task 1", IsCompleted = false },
            new TodoItem { Id = Guid.NewGuid(), Title = "Task 2", IsCompleted = true }
        };

        var mockDbSet = new Mock<DbSet<TodoItem>>();
        mockDbSet.As<IQueryable<TodoItem>>().Setup(m => m.Provider).Returns(todoItems.AsQueryable().Provider);
        mockDbSet.As<IQueryable<TodoItem>>().Setup(m => m.Expression).Returns(todoItems.AsQueryable().Expression);
        mockDbSet.As<IQueryable<TodoItem>>().Setup(m => m.ElementType).Returns(todoItems.AsQueryable().ElementType);
        mockDbSet.As<IQueryable<TodoItem>>().Setup(m => m.GetEnumerator()).Returns(todoItems.GetEnumerator());

        var mockContext = new Mock<AppDbContext>();
        mockContext.Setup(c => c.TodoItems).Returns(mockDbSet.Object);

        var todoService = new TodoService(mockContext.Object);

        // Act
        var result = todoService.GetAllTodoItems();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public void DeleteTodoItem_ShouldRemoveItemFromDatabase()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var todoItem = new TodoItem { Id = itemId, Title = "Test Task", IsCompleted = false };

        var mockDbSet = new Mock<DbSet<TodoItem>>();
        var mockContext = new Mock<AppDbContext>();
        mockContext.Setup(c => c.TodoItems).Returns(mockDbSet.Object);

        var todoService = new TodoService(mockContext.Object);

        // Act
        todoService.DeleteTodoItem(itemId);

        // Assert
        mockDbSet.Verify(m => m.Remove(It.IsAny<TodoItem>()), Times.Once());
        mockContext.Verify(m => m.SaveChanges(), Times.Once());
    }
}