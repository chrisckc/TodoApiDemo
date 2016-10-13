using System;
using System.Collections.Generic;
using TodoApi.Models;

namespace TodoApi.Data
{
    public interface ITodoRepository
    {
        void Add(TodoItem item);
        IEnumerable<TodoItem> GetAll();
        TodoItem Find(Guid id);
        TodoItem Remove(Guid id);
        void Update(TodoItem item);
    }
}