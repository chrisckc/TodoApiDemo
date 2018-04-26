using System;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models.Responses;
using TodoApi.Models.DataTransferObjects;

namespace TodoApi.Filters
{
    /// <summary>
    /// This represents the filter entity for global exceptions.
    /// </summary>
    public class GlobalExceptionFilter : IExceptionFilter, IDisposable
    {
        private readonly ILogger _logger;

        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalExceptionFilter"/> class.
        /// </summary>
        /// <param name="logger"><see cref="ILoggerFactory"/> instance.</param>
        public GlobalExceptionFilter(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            this._logger = loggerFactory.CreateLogger("Global Exception Filter");
        }

        /// <summary>
        /// Performs while an exception arises.
        /// </summary>
        /// <param name="context"><see cref="ExceptionContext"/> instance.</param>
        public void OnException(ExceptionContext context)
        {
            var errorResponse = new ErrorResponse() { Message = context.Exception.Message };
            Exception exception = context.Exception;
            var exceptionDto = GetExceptionDto(exception);
            while (exception.InnerException != null)
            {
                exceptionDto.InnerException = GetExceptionDto(exception.InnerException);
                exception = exception.InnerException;
            }

            errorResponse.Exception = exceptionDto;
            context.Result = new ObjectResult(errorResponse)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                DeclaredType = typeof(ErrorResponse)
            };

            this._logger.LogError("Exception: {exception}", context.Exception);
        }

        private ExceptionDto GetExceptionDto(Exception exception) 
        {
            var exceptionDto = new ExceptionDto() 
                {   Message = exception.Message,
                    Type = exception.GetType().ToString(),
                    Source = exception.Source
                };
#if DEBUG
            exceptionDto.StackTrace = exception.StackTrace;
#endif
            return exceptionDto;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this._disposed)
            {
                return;
            }

            this._disposed = true;
        }
    }
}

