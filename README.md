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

The exception does but not bubble up to the controller, the status code is 200 OK


##Update:
Another test has been added
perform a GET request to:
http://localhost:5000/api/api/Values/NotSerializable

This has a similar result with a different kind of serialization error
