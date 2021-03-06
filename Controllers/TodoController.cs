﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using TodoApi.Models;
using TodoApi.Data;
using TodoApiDemo.Logging;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        public ITodoRepository _todoRepository { get; set; }
        private readonly ILogger<TodoController> _logger;
        
        public TodoController(ITodoRepository todoRepository, ILogger<TodoController> logger)
        {
            _todoRepository = todoRepository;
            _logger = logger;
        }


        #region snippet_GetAll
        [HttpGet] // Matches '/Todo/'
        public IEnumerable<TodoItem> GetAll()
        {
            _logger.LogInformation(LogEvents.GetItem, "GetAll()");
            return _todoRepository.GetAll();
        }
        #endregion

        [HttpGet("{id}", Name = "GetTodo")] // Matches '/Todo/{id}'
        public IActionResult GetById(Guid id)
        {
            _logger.LogInformation(LogEvents.GetItem, "GetById({ID})", id);
            var item = _todoRepository.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }


       [HttpGet("{id}/Notes")] // Matches '/Todo/{id}/Notes'
        public IActionResult GetNotes(Guid id)
        {
            _logger.LogInformation(LogEvents.GetItem, "GetNotes({ID})", id);
            var item = _todoRepository.Find(id);

            if (item == null)
            {
                return NotFound();
            } 
            else
            {
                ICollection<Note> array = item.Notes;
                _logger.LogInformation("Note array size: {array.Count}",array.Count);
                //When the above is serialized only 1 of the 3 note objects are serialized, but the the status code is still 200 indicating a successful response...
                //Also, neither the developer exception page or the GlobalExceptionFilter catch it.
                //Uncomment the [JsonIgnore] attribute in the Models/Note.cs file to remove the reference loop and see the above working correctly outputting all 3 notes.
                //Or you can edit the startup.cs and uncomment ReferenceLoopHandling.Ignore 
                
                return Ok(array); //comment this line out and uncomment the below code block to see the controller behave as expected and throw an exception with 500 status code
               

                //This code demonstrates the exception occurring inside the controller 
                //and 500 status code correctly being returned
                //
                // JsonSerializerSettings settings = new JsonSerializerSettings {
                //     ContractResolver = new CamelCasePropertyNamesContractResolver(),
                //     Formatting = Formatting.Indented,
                //     NullValueHandling = NullValueHandling.Include,
                //     MissingMemberHandling = MissingMemberHandling.Ignore,
                //     //ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                // };
                // settings.Converters.Add(new StringEnumConverter());
                // string jsonString = JsonConvert.SerializeObject(array,settings);
                // return Ok(jsonString);
                
            }
        }

        #region snippet_Create
        [HttpPost]
        public IActionResult Create([FromBody] TodoItem item)
        {
            if (item == null)
            {
                return BadRequest();
            }
            _todoRepository.Add(item);
            return CreatedAtRoute("GetTodo", new { id = item.TodoItemId }, item);
        }
        #endregion

        #region snippet_Update
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] TodoItem item)
        {
            if (item == null || item.TodoItemId != id)
            {
                return BadRequest();
            }

            var todo = _todoRepository.Find(id);
            if (todo == null)
            {
                return NotFound();
            }

            _todoRepository.Update(item);
            return new NoContentResult();
        }
        #endregion

        //This is not implementing correct HTTP PATCH as per the RFC
        #region snippet_Patch
        [HttpPatch("{id}")]
        public IActionResult Update([FromBody] TodoItem item, Guid id)
        {
            if (item == null)
            {
                return BadRequest();
            }

            var todo = _todoRepository.Find(id);
            if (todo == null)
            {
                return NotFound();
            }

            item.TodoItemId = todo.TodoItemId;

            _todoRepository.Update(item);
            return new NoContentResult();
        }
        #endregion

        #region snippet_Delete
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var todo = _todoRepository.Find(id);
            if (todo == null)
            {
                return NotFound();
            }

            _todoRepository.Remove(id);
            return new NoContentResult();
        }
        #endregion
    }
}
