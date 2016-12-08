using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class RequestLogger
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLogger> _logger;
 
    public RequestLogger(RequestDelegate next, ILogger<RequestLogger> logger)
    {
        _next = next;
        _logger = logger;
    }
 
    public async Task Invoke(HttpContext context)
    {
        DateTime startTime = DateTime.UtcNow;

        //Log the Request
        await LogRequest(context, startTime);
 
        // Start the stopwatch after we have logged the request as we are not interested in the time taken to do the logging
        var sw = Stopwatch.StartNew();
        DateTime endTime = DateTime.MinValue;

        // Setup the OnStarting callback so that we stop the stopwatch when the response starts to be sent to the client
        context.Response.OnStarting((state) =>
        {
            sw.Stop();
            endTime = startTime.AddMilliseconds(sw.ElapsedMilliseconds);
            context.Response.Headers.Add("X-Processing-Time", sw.ElapsedMilliseconds.ToString());            
            return Task.FromResult(0);
        }, null);

        // Instead we can log the response after it has finished being sent to the client is we want to ...
        // but we are not interested in including the network latency here
        // context.Response.OnCompleted(() =>
        // {
        //     LogResponse(context, endTime, sw.ElapsedMilliseconds);
        //     return Task.FromResult(0);
        // });

        string errorMessage = null;
        try
        {
            //Wait for the rest of the Middlewares to complete
            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
            throw ex;
        }
        finally
        {
            // Stop the stopwatch just in case this code is reached before the OnStarting callback
            sw.Stop();
            endTime = startTime.AddMilliseconds(sw.ElapsedMilliseconds);

            // Add the processing time to the headers
            // We can't do this as the response has already started at this point
            //context.Response.Headers.Add("X-Processing-Time", new[] { sw.ElapsedMilliseconds.ToString() });

            //Log the Response
            LogResponse(context, endTime, sw.ElapsedMilliseconds);

            //If we have an error then log that also
            if (errorMessage != null) {
                _logger.LogCritical("Exception: {0}", errorMessage);
                // Maybe try to write the error text to the end of the body
                // this is not going to work though the connection will be probably be closed by now
                //byte[] data = System.Text.Encoding.UTF8.GetBytes(errorMessage);
                //await context.Response.Body.WriteAsync(data, 0, data.Length);
            }
        }
    }

    private async Task LogRequest(HttpContext context, DateTime startTime)
    {
        String requestHeaders = "";
        int requestHeaderCounter = 1;
        foreach (string key in context.Request.Headers.Keys) {
            requestHeaders += string.Format("Request header{0}: {1} = {2}\r\n", requestHeaderCounter, key, context.Request.Headers[key]);
            requestHeaderCounter++;
        }
        requestHeaders = requestHeaders.TrimEnd('\r', '\n');
 
        var requestLogTemplate = @"
Request Start time: {0}
Request client IP: {1}
Request method: {2}
Request path: {3}
Request headers:
{4}";
        string requestLog = string.Format(requestLogTemplate,
            startTime,
            context.Connection.RemoteIpAddress.ToString(),
            context.Request.Method,
            context.Request.Path,
            requestHeaders);

        //Log the Request body
        if (true) // TODO: make this a config option...
        {
            string requestBody;
            using (var bodyReader = new StreamReader(context.Request.Body))
            {
                requestBody = await bodyReader.ReadToEndAsync();
                context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));
            }
            requestLog += string.Format("\r\nRequest body:\r\n{0}", requestBody);
        }

        //Write to the log
        _logger.LogInformation(requestLog);
    }

    private void LogResponse(HttpContext context, DateTime endTime, long duration)
    {
        string responseHeaders = "";
        int responseHeaderCounter = 1;
        foreach (string key in context.Response.Headers.Keys) {
            responseHeaders += string.Format("Response Header{0}: {1} = {2}\r\n", responseHeaderCounter, key, context.Response.Headers[key]);
            responseHeaderCounter++;
        }
        responseHeaders = responseHeaders.TrimEnd('\r', '\n');

        var responseLogTemplate = @"
Response end time: {endTime}
Response duration: {duration}
Response status code: {statusCode}
Response headers:
{responseHeaders}";

        //Write to the log
        _logger.LogInformation(responseLogTemplate,
            endTime,
            duration,
            context.Response.StatusCode,
            responseHeaders);
    }

}