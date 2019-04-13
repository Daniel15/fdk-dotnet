# Fn Function Developer Kit for C\#

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

There are three possible ways to configure Fn functions. These are similar to how [ASP.NET Core Middleware](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write) can be written:

## Anonymous Function

One approach is to call `FdkHandler.Handle` with a [lambda expression](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/lambda-expressions). The function takes two arguments: A context containing information about the request (for example, HTTP headers, app name, function name, etc), and the input to the function. The function can be either a regular function, or an asynchronous function using the `async` keyword.

```csharp
FdkHandler.Handle((ctx, input) =>
{
    var name = input.AsString();
    return $"Hello {name}";
});
```

## Class

Alternatively, the function can be configured as a class. This is what `fn init` will give you out of the box, and is generally the recommended approach. Instead of taking the `IInput`, you can just have a `string` argument directly. Your class must have either an `InvokeAsync` or `Invoke` method.

```csharp
class MyFunction
{
    public async Task<string> InvokeAsync(string input, CancellationToken timedOut)
    {
        // ...you could do some async stuff here...
        return Task.FromResult($"Hello {input}");
    }
}
```

The `CancellationToken` is important to ensure the function is properly aborted if it exceeds [the configured function call timeout](https://github.com/fnproject/docs/blob/master/fn/develop/function-timeouts.md#function-call-timeout). You should pass it to all async methods called by your function.

For functions that do not do any async operations, you can implement `Invoke` instead:

```csharp
class MyFunction
{
    public string Invoke(string input)
    {
        return $"Hello {input}";
    }
}
```

## `IFunction` Interface

You can also implement the `IFunction` interface directly. However, this is not recommended as it has extra boilerplate:

```csharp
public class FooFunction : IFunction
{
    public Task<object> InvokeAsync(IContext ctx, IInput input, IServiceProvider services)
    {
        var name = input.AsString();
        return $"Hello {name}";
    }
}
```

## Input

### JSON

To get the input as a strongly-typed JSON object, just add the object as a parameter to your function:

```csharp
public class MyInput
{
    public string Name { get; set; }
}

class MyFunction
{
    public string Invoke(MyInput input)
    {
        return $"Hello {input.Name}!";
    }
}
```

Alternatively, you can use `dynamic` to get the data as a [dynamic object](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/types/walkthrough-creating-and-using-dynamic-objects):

```csharp
class MyFunction
{
    public string Invoke(dynamic input)
    {
        return $"Hello {input.Name}!";
    }
}
```

If using the lambda expression syntax, use the `AsJson` method on the `input` argument:

```csharp
FdkHandler.Handle((ctx, input) =>
{
    var inputObj = input.AsJson<MyInput>();
    return $"Hello {input.Name}!";
});
```

### Headers

Request headers can be obtained from the `Headers` property on the `IContext`:

```csharp
class MyFunction
{
    public string Invoke(IContext ctx)
    {
        var foo = ctx.Headers["X-Foo"];
        // ...
    }
}
```

`IContext` also contains other information about the request, such as the call ID (which is unique per call), the HTTP method, the system configuration, etc.

## Output

### Raw

To return "raw" data from your function, either return a string (as shown above), or a stream:

```csharp
class MyFunction
{
    public Stream Invoke()
    {
        return File.Open("/tmp/test.txt", FileMode.Open, FileAccess.Read);
    }
}
```

A stream is useful when returning a large response, as the entire response does not need to be cached in RAM beforehand. The stream will be automatically closed and disposed once the request completes.

### JSON
Any POCOs (Plain Old C# Objects) returned from the function will be automatically serialized as JSON. This can be an anonymous type:

```csharp
class MyFunction
{
    public object Invoke()
    {
        return new { Message = "Hello World!" };
    }
}
```

Example of invoking this function:

```sh
$ fn invoke Test myfunction
{"Message":"Hello World!"}
```

## Using HTTP headers and setting HTTP status codes

To set custom status codes or headers, you need to return a subclass of `FnResult` (`RawResult`, `JsonResult` or `StreamResult`, depending on the type you're returning, as documented above). This contains `HttpStatus` and `Headers` properties:

```csharp
class MyFunction
{
    public object Invoke(string input)
    {
        return new RawResult($"Hello {input}!")
        {
            // Example of custom status code
            HttpStatus = StatusCodes.Status202Accepted,
            // Example of custom headers
            Headers =
            {
                ["X-Some-Header"] = "foo"
            }
        };
    }
}
```

## Timeouts

To properly handle [function call timeouts](https://github.com/fnproject/docs/blob/master/fn/develop/function-timeouts.md#function-call-timeout), any functions that perform I/O (database operations, downloading/uploading, etc) should be `async` functions, and use the `CancellationToken` provided as an argument (or in `ctx.TimedOut` for the lambda expression syntax) for all their async function calls. For example:

```csharp
var client = new HttpClient();
var response = await client.GetAsync("https://www.example.com/", ctx.TimedOut);
```

This will ensure that the operation is correctly cancelled if the function times out.
