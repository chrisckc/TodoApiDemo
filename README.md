# TodoApiDemo

Based on the TodoApi provided in the aspnet documentation, created to demonstrate a serialization issue.

## Notes

To reproduce the serialization issue:

Run this code on a Mac using either dotnet run or VSCode debug session.
haven't tried this on Windows in VS2015 yet

perform a GET request to:
http://localhost:5000/api/todo/10000000-1111-1111-1111-111111111111/Notes

That url routes to Controllers/TodoController.cs    method: GetNotes(Guid id)

The method returns the notes for the todo item (REST style), there should be 3 notes returned.
However when the array of 3 notes is serialized only 1 of the 3 note objects is actually serialized
due to an exception occurring in json.net when serializing the first note (as would be expected due to the self referencing loop).

The exception does but not bubble up to the controller, the status code is 200 and we have valid Json (but missing the other 2 notes)
indicating a successful response, so it would be hard for a client to know there was a problem.

Also, neither the developer exception page or the GlobalExceptionFilter is able catch it.

Note the following lines in the console log output:

Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware:Error: An unhandled exception has occurred while executing the request
Newtonsoft.Json.JsonSerializationException: Self referencing loop detected for property 'notes' with type 'System.Collections.Generic.List`1[TodoApi.Models.Note]'. Path ‘[0].todoItem'.

Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware:Warning: The response has already started, the error page middleware will not be executed.
Microsoft.AspNetCore.Server.Kestrel:Error: Connection id "0HKVJ34SUMOMU": An unhandled exception was thrown by the application.
Newtonsoft.Json.JsonSerializationException: Self referencing loop detected for property 'notes' with type 'System.Collections.Generic.List`1[TodoApi.Models.Note]'. Path ‘[0].todoItem'.

Microsoft.AspNetCore.Hosting.Internal.WebHost:Information: Request finished in 91.7335ms 200 application/json; charset=utf-8


##Update:
Another test has been added
perform a GET request to:
http://localhost:5000/api/api/Values/NotSerializable

This has a similar result with a different kind of serialization error
