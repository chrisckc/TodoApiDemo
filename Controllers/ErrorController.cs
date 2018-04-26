using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models.Responses;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoApiDemo.Controllers
{
    [Route("api/[controller]")]
    public class ErrorController : Controller
    {
        // GET: api/error
        public ErrorResponse Get()
        {
            return new ErrorResponse
            {
                Code = 0,
                Message = "An Unhandled Exception was encountered",
                UserMessage = "An Error occured in the API",
                MoreInfo = ""
            };
        }
    }
}
