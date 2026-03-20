using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;
using TodoApi.Services;
using Xunit;

namespace TodoApi.Tests
{
    public class TodoServiceTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public void Add_Todo_Should_Work()
        {
            var context = GetDbContext();
            var service = new TodoService(context);

            var result = service.Add(new TodoItem { Title = "Test" });

            Assert.NotNull(result);
            Assert.Equal(1, context.Todos.Count());
        }

        [Fact]
        public void GetAll_Should_Return_Items()
        {
            var context = GetDbContext();
            context.Todos.Add(new TodoItem { Title = "Test" });
            context.SaveChanges();

            var service = new TodoService(context);
            var result = service.GetAll();

            Assert.Single(result);
        }

        [Fact]
        public void Delete_Should_Remove_Item()
        {
            var context = GetDbContext();
            context.Todos.Add(new TodoItem { Id = 1, Title = "Test" });
            context.SaveChanges();

            var service = new TodoService(context);
            var deleted = service.Delete(1);

            Assert.True(deleted);
            Assert.Empty(context.Todos);
        }
    }
}