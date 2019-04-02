# Fn Function Developer Kit for C#

This Function Developer Kit makes it easy to deploy .NET Core functions to Fn.

# Quick Start

Before starting, ensure you have installed Fn, and started a local Fn server by running `fn start`.

```sh
# Create new function
mkdir MyFunction
cd MyFunction
fn init --init-image daniel15/fn-dotnet-init

# Deploy it to local Fn server (make sure you've ran "fn start" first)
fn deploy --app Test --create-app --local

# Test it out!
echo "Daniel" | fn invoke Test MyFunction
# Should output "Hello Daniel"
```

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
