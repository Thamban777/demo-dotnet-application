using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Services
{
    public class TodoService : ITodoService
    {
        private readonly AppDbContext _context;

        public TodoService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<TodoItem> GetAll()
        {
            return _context.Todos.ToList();
        }

        public TodoItem? GetById(int id)
        {
            return _context.Todos.Find(id);
        }

        public TodoItem Add(TodoItem item)
        {
            _context.Todos.Add(item);
            _context.SaveChanges();
            return item;
        }

        public bool Update(int id, TodoItem item)
        {
            var existing = _context.Todos.Find(id);
            if (existing == null) return false;

            existing.Title = item.Title;
            existing.IsCompleted = item.IsCompleted;

            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var item = _context.Todos.Find(id);
            if (item == null) return false;

            _context.Todos.Remove(item);
            _context.SaveChanges();
            return true;
        }
    }
}