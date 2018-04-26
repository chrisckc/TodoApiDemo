using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TodoApi.Models.DataTransferObjects;
using TodoApi.Models.Responses;
using TodoApiDemo.Extensions;
using TodoApiDemo.Logging;

namespace TodoApiDemo.Handlers
{
    public class ExceptionHandler : IExceptionHandler
    {
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _userMessage;
        public bool IncludeException { get; set; }
        public bool IncludeExceptionStacktrace { get; set; }
        private const int _errorCode = 1;
        private const int _exceptionCode = 0;


        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandler"/> class.
        /// </summary>
        /// <param name="logger"><see cref="ILoggerFactory"/> instance.</param>

        public ExceptionHandler(IHttpContextAccessor httpContextAccessor, ILogger<ExceptionHandler> logger, bool includeException = false, bool includeExceptionStacktrace = false)
        {
            if (httpContextAccessor == null)
            {
                throw new ArgumentNullException(nameof(httpContextAccessor));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _userMessage = "An Unhandled Exception Occurred in the API!";
            this.IncludeException = includeException;
            this.IncludeExceptionStacktrace = includeExceptionStacktrace;
        }

        /// <summary>
        /// Handles an exception.
        /// </summary>
        /// <param name="context"><see cref="HttpContext"/> instance.</param>
        public async Task HandleException(HttpContext context)
        {
            int statusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            var ex = context.Features.Get<IExceptionHandlerFeature>();
            var errorResponse = new ErrorResponse {UserMessage = this._userMessage, TimeStamp = DateTime.Now};
            string requestPath = _httpContextAccessor?.HttpContext?.Request?.Path.ToString() ?? "No path";
            errorResponse.RequestPath = requestPath;
            string method = _httpContextAccessor?.HttpContext?.Request?.Method?.ToString() ?? "Unknown";
            errorResponse.RequestMethod = method;
            errorResponse.StatusCode = statusCode;
            Exception exception = null;
            if (ex != null)
            {
                exception = ex.Error;
                if (exception != null && exception.Message != null) {
                    errorResponse.Message = exception.Message.Truncate(60);
                }
            }
            if (exception != null && this.IncludeException)
            {
                errorResponse.Exception = GetExceptionDtoTree(exception, this.IncludeExceptionStacktrace);
            }
            string json = SerializeObject(errorResponse);
            // write the response
            await context.Response.WriteAsync(json).ConfigureAwait(false);

            // always log the full exception with stack trace
            if (exception != null) {
                if (this.IncludeException && this.IncludeExceptionStacktrace) {
                    // nothing tp do as we already have the full details
                } else {
                    // if the exception was not included or did not include the stack trace recreate it
                    errorResponse.Exception = GetExceptionDtoTree(exception, true);
                }
            }
            string strErrorResponse = SerializeObject(errorResponse); 
            this._logger.LogError(LogEvents.Exception, "UnhandledException:\n{strErrorResponse}", strErrorResponse);
        }

        private ExceptionDto GetExceptionDtoTree(Exception exception, bool includeStacktrace)
        {
            var exceptionDto = GetExceptionDto(exception, includeStacktrace);

            // walk through the inner exceptions...
            while (exception.InnerException != null)
            {
                exceptionDto.InnerException = GetExceptionDto(exception.InnerException, includeStacktrace);
                exception = exception.InnerException;
            }
            return exceptionDto;
        }


        private ExceptionDto GetExceptionDto(Exception exception, bool includeStacktrace)
        {
            var exceptionDto = new ExceptionDto()
            {
                Message = exception.Message,
                Type = exception.GetType().ToString(),
                Source = exception.Source
            };
            if (includeStacktrace) {
                exceptionDto.StackTrace = exception.StackTrace;
            }
            return exceptionDto;
        }

        private string SerializeObject(Object errorResponse) {
            return  JsonConvert.SerializeObject(errorResponse, Formatting.Indented,
                        new JsonConverter[] { new StringEnumConverter() });

        }
    }
}
