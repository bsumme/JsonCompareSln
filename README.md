This project contains the JSON Comparer Class library that compares two JSON strings and returns a new JSON string containing the all the keys that are shared by the two strings but have different values. It also returns those differing values

ie. 
string jsonstring1 = "{"name":"John","age":30}";
string jsonstring2 = "{"name":"John","age":35, "height":61}";

return jsonString would be 
{"age":30,35}


To build this project download the code to VSCode and then open terminal and run 
dotnet build

This should build the project both in .net461 and net6.0 windows

You can run this project using the TestRunner console app by running
dotnet run --project TestRunner\TestRunner.csproj --framework net461
or
dotnet run --project TestRunner\TestRunner.csproj --framework net6.0-window


This project is meant to be class library that can imported into UIPath as a custom library that can be invoked as a Custom Activity.
The class library is JsonComparer.dll that after build is located in the following paths.
bin\Debug\net461\JsonComparer.dll (used for windows legacy UIPath projects)
bin\Debug\net461\JsonComparer.dll (used for current windows UIPath projects)

To package this class library .dll for use in UiPath you should follow instructions here
Look at Step 3: Create a NuGet package
https://docs.uipath.com/activities/other/latest/developer/migrating-activities-to-net


Other useful links to build similar project
https://learn.microsoft.com/en-us/dotnet/core/tutorials/library-with-visual-studio-code?pivots=dotnet-7-0
https://docs.uipath.com/activities/other/latest/developer/creating-activities-with-code