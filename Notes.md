## Notes

#Packages added
dotnet add package Microsoft.AspNetCore.ResponseCompression
dotnet add package Microsoft.AspNetCore.Buffering


#environment variables can be used:

EnvironmentName // the name of the config environment eg development, production etc.
ApplicationName // the name of the assembly


##Post migration steps not mentioned in the official docs:

#csproj file:

update:
<RuntimeFrameworkVersion>2.0.0</RuntimeFrameworkVersion>
or just delete the entry

add:
  <ItemGroup>
    <DotNetCliToolReference Include="Microsoft.DotNet.Watcher.Tools" Version="2.0.0" />
  </ItemGroup>


In Program.cs , add using for AspNetCore
using Microsoft.AspNetCore;


Using launch settings from /TodoApiDemo/Properties/launchSettings.json an error occurred:
The launch profile "(Default)" could not be applied.
A usable launch profile could not be located.

fix: delete the file: properties/launchSettings.json

When running in vscode, got error:
The specified framework 'Microsoft.NETCore.App', version '1.0.4' was not found.
Check application dependencies and target a framework version installed at...

To get it working in vscode:
delete .vscode/launch.json
delete .vscode/tasks.json

Let vscode offer to recreate the files via the debug menu

In launch.json, edit the "program" property in the 2 elements of the "configurations" array entries to match the path of the assembly dll by combining the framework version "netcoreapp2.0" and the name of the assembly dll.

Error: Could not find the preLaunchTask 'build'.
Let vscode recreate the tasks.json file, click configure task and choose the .NET Core template


Build self contained executable for Raspberry Pi
dotnet publish -c release -o published -r linux-arm