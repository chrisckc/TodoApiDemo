using System;
using Newtonsoft.Json;
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

        public DateTime TimeStamp { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string RequestPath { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string RequestMethod { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int StatusCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MoreInfo { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ExceptionDto Exception { get; set; }

    }
}
