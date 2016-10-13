using TodoApi.Models.DataTransferObjects;

namespace TodoApi.Models.Responses
{
    /// <summary>
    /// This represents the response entity for error.
    /// </summary>
    public class ErrorResponse
    {
        public int Code { get; set; }

        public string Message { get; set; }

        public string UserMessage { get; set; }

        public string MoreInfo { get; set; }

        public ExceptionDto Exception { get; set; }

    }
}
