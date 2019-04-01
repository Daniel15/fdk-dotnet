# Fn Function Developer Kit for C#

This Function Developer Kit makes it easy to deploy .NET Core functions to Fn.

# Quick Start

Before starting, ensure you have installed Fn, and that it's working as expected by completing one of the official tutorials (for example, the [Node.js tutorial](https://fnproject.io/tutorials/node/intro/)).

Create a new .NET Core console app and add the `FnProject.Fdk` NuGet package. You can use your favourite IDE (eg. Visual Studio, VS Code), or the `dotnet` command line:
```sh
mkdir MyFunction
cd MyFunction
dotnet new console
dotnet add package FnProject.Fdk
```

Update `Program.cs`:
```csharp
using FnProject.Fdk;
using System;

namespace MyFunction
{
    class Program
    {
        static void Main(string[] args)
        {
            FdkHandler.Handle((ctx, input) =>
            {
                return "Hello " + input.AsString();
            });
        }
    }
}
```

Create `Dockerfile`:
```dockerfile
FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["MyFunction.csproj", "."]
RUN dotnet restore "MyFunction.csproj"
COPY . .
RUN dotnet build "MyFunction.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "MyFunction.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MyFunction.dll"]
```

Create `func.yaml`:
```yaml
schema_version: 20180708
name: myfunction
version: 0.0.1
runtime: docker
triggers:
- name: myfunction-trigger
  type: http
  source: /myfunction
```

Deploy the function:
```sh
fn --verbose deploy --app Test --create-app --local
```

Invoke it and ensure it works:
```sh
echo "Daniel" | fn invoke Test myfunction
```
This should return "Hello Daniel"

# API

To configure your function, call `FdkHandler.Handle` with a lambda function. The function takes two arguments: A context containing information about the request (for example, HTTP headers, app name, function name, etc), and the input to the function.

## Input
The following methods are available to obtain input for your function:

### AsString
Get the input as a string

### AsJson
Parse the input as a JSON object, either as a dynamic object:
```csharp
FdkHandler.Handle((ctx, input) =>
{
	dynamic inputObj = input.AsJson();
	return "Hello " + input.Name;
});
```
or as a strongly-typed object:
```csharp
public class MyInput {
	public string Name { get; set; }
}
...

FdkHandler.Handle((ctx, input) =>
{
	var inputObj = input.AsJson<MyInput>();
	return "Hello " + input.Name;
});
```

## Output

### Raw
To return "raw" data from your function, either return a string:
```csharp
FdkHandler.Handle((ctx, input) =>
{
	return "Hello World!";
});
```

Or a stream:
```csharp
FdkHandler.Handle((ctx, input) =>
{
	return File.Open("/tmp/test.txt", FileMode.Open, FileAccess.Read);
});
```
A stream is useful when returning a large response, as the entire response does not need to be cached in RAM beforehand. The stream will be automatically closed and disposed once the request completes.

### JSON
Any POCOs (Plain Old C# Objects) returned from the function will be automatically serialized as JSON. This can be an anonymous type:
```csharp
FdkHandler.Handle((ctx, input) =>
{
	return new { Message = "Hello World!" };
});
```

Example of invoking this function:
```sh
$ fn invoke Test myfunction
{"Message":"Hello World!"}
```
