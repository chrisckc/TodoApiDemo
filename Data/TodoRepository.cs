using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using TodoApi.Models;
using TodoApiDemo.Logging;

namespace TodoApi.Data
{
    public class TodoRepository : ITodoRepository
    {
        private readonly ILogger<TodoRepository> _logger;
        private static ConcurrentDictionary<Guid, TodoItem> _todos =
              new ConcurrentDictionary<Guid, TodoItem>();

        private static bool isInitialized = false;
        private static int todoCount  = 174; // Number of todo items to generate
        //private static int todoCount  = 17; // Number of todo items to generate

        public TodoRepository(ILogger<TodoRepository> logger)
        {
            _logger = logger;
    
            // this will run the first time the api is called
            if (!isInitialized) {
                _logger.LogInformation(LogEvents.Info, "Creating seed data...");
                //Seed it with some data...
                TodoItem firstTodo = new TodoItem { TodoItemId = new Guid("10000000-1111-1111-1111-111111111111"),
                                                TodoItemName = "Item1" };
                //Create a list of notes and set the reverse navigation property just like EF would
                //when related objects are loaded.
                TodoItem firstTodoItem = null;
                //TodoItem firstTodoItem = firstTodo;
                List<Note> firstNotes = new List<Note> () 
                                    {   new Note { NoteText = "Item1 test note 1", TodoItem = firstTodoItem },
                                        new Note { NoteText = "Item1 test note 2", TodoItem = firstTodoItem },
                                        new Note { NoteText = "Item1 test note 3", TodoItem = firstTodoItem } 
                                    };
                //Assign to the todo item
                firstTodo.Notes = firstNotes;
                Add(firstTodo);

                for (int i = 2; i <= todoCount; i++)
                {
                    //Console.WriteLine(i);
                    TodoItem todo = new TodoItem { TodoItemId = new Guid(), TodoItemName = $"Item{i}" };
                    TodoItem todoItem = null;
                    //TodoItem todoItem = todo;
                    List<Note> notes = new List<Note> () 
                                        {   new Note { NoteText = $"Item{i} test note 1", TodoItem = todoItem },
                                            new Note { NoteText = $"Item{i} test note 1", TodoItem = todoItem },
                                            new Note { NoteText = $"Item{i} test note 1", TodoItem = todoItem } 
                                        };
                    //Assign to the todo item
                    todo.Notes = notes;
                    Add(todo);
                }
                isInitialized = true;
            }           
            
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