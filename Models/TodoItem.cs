using System;
using System.Collections.Generic;

namespace TodoApi.Models
{
    public class TodoItem
    {
        public TodoItem()
        {
            this.Notes = new HashSet<Note>();
        }
        
        public Guid TodoItemId { get; set; }
        public string TodoItemName { get; set; }
        public bool IsComplete { get; set; }

        //Collections
        public virtual ICollection<Note> Notes { get; set; }
    }
}