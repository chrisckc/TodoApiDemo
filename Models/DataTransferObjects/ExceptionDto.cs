
namespace TodoApi.Models.DataTransferObjects
{
    /// <summary>
    /// This represents the response entity for error.
    /// </summary>
    public class ExceptionDto
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the stack trace.
        /// </summary>
#if !DEBUG
        [JsonIgnore]
#endif
        public string StackTrace { get; set; }

                /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public ExceptionDto InnerException { get; set; }
    }
}
