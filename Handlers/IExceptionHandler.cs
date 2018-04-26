using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TodoApiDemo.Handlers
{
    public interface IExceptionHandler
    {
        bool IncludeException { get; set; }
        bool IncludeExceptionStacktrace { get; set; }
        Task HandleException(HttpContext context);
    }
}
