using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using TodoApi.Models;

namespace TodoApi.Data
{
    public class TodoRepository : ITodoRepository
    {
        private static ConcurrentDictionary<Guid, TodoItem> _todos =
              new ConcurrentDictionary<Guid, TodoItem>();

        public TodoRepository()
        {
            //Seed it with some data...
            TodoItem todo = new TodoItem { TodoItemId = new Guid("10000000-1111-1111-1111-111111111111"),
                                            TodoItemName = "Item1" };
            //Create a list of notes and set the reverse navigation property just like EF would
            //when related objects are loaded.
            List<Note> notes = new List<Note> () 
                                {   new Note { NoteText = "test note 1", TodoItem = todo },
                                    new Note { NoteText = "test note 2", TodoItem = todo },
                                    new Note { NoteText = "test note 3", TodoItem = todo } 
                                };
            //Assign to the todo item
            todo.Notes = notes;
            Add(todo);

        }

        public IEnumerable<TodoItem> GetAll()
        {
            return _todos.Values;
        }

        public void Add(TodoItem item)
        {
            if (item.TodoItemId == Guid.Empty) item.TodoItemId = Guid.NewGuid();

            foreach (Note note in item.Notes) {
                if (note.NoteId == Guid.Empty) note.NoteId = Guid.NewGuid();
            }
            _todos[item.TodoItemId] = item;
        }

        public TodoItem Find(Guid id)
        {
            TodoItem item;
            _todos.TryGetValue(id, out item);
            return item;
        }

        public TodoItem Remove(Guid id)
        {
            TodoItem item;
            _todos.TryRemove(id, out item);
            return item;
        }

        public void Update(TodoItem item)
        {
            _todos[item.TodoItemId] = item;
        }
    }
}