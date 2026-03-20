using TodoApi.Models;

namespace TodoApi.Services
{
    public interface ITodoService
    {
        IEnumerable<TodoItem> GetAll();
        TodoItem? GetById(int id);
        TodoItem Add(TodoItem item);
        bool Update(int id, TodoItem item);
        bool Delete(int id);
    }
}