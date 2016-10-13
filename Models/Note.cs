using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace TodoApi.Models
{
    public class Note
    {
        public Guid NoteId { get; set; }
        public string NoteText { get; set; }

        //Navigation Properties
        //[JsonIgnore]  //Uncommenting this works around the issue by preventing the self referencing loop
        public virtual TodoItem TodoItem { get; set; }


    }
}