# FaasUtils

[![Build Status](https://dev.azure.com/daniel15/fdk-dotnet/_apis/build/status/Daniel15.fdk-dotnet?branchName=master)](https://dev.azure.com/daniel15/fdk-dotnet/_build/latest?definitionId=5&branchName=master)&nbsp;
[![NuGet version](http://img.shields.io/nuget/v/FaasUtils.svg)](https://www.nuget.org/packages/FaasUtils/)

`FaasUtils` is a collection of utilities that could be useful for various FaaS systems/providers (FnProject, OpenFaaS, etc). It is designed to be used as a dependency for FaaS SDKs, rather than directly by functions themselves.

It consists of two main components:

## Function Input

`IInput` is an interface representing the input to a FaaS function. This contains several methods to obtain the input:

- `AsString`: Raw input as a string
- `AsJson<T>`: Parses the input as a strongly-typed JSON object
- `AsJson`: Parses the input as a dynamic JSON object
- `AsStream` Raw input as a stream (ideal for large inputs, or binary content such as image files)

It comes with a `Input` class that implements `IInput` by wrapping `HttpContext`. Additional implementations could be provided in order to implement different trigger types.

## Function Calls

The main component of `FaasUtils` is `FunctionExpressionTreeBuilder`. This class can produce a lambda function for any arbitrary class containing an `Invoke` or `InvokeAsync` method, similar to how [ASP.NET middleware classes](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write?view=aspnetcore-2.2) work. The generated lambda function handles resolving interfaces through an `IServiceProvider`.

As an example, given a function like this:

```csharp
class MyFunction
{
    public async Task<string> InvokeAsync(string input, IFoo foo)
    {
        return $"Hello {input}";
    }
}
```

Calling `FunctionExpressionTreeBuilder.Compile` will compile a lambda function roughly like this:

```csharp
(MyFunction instance, IServiceProvider services) => instance.Invoke(
    services.GetRequiredService<IInput>().AsString,
    services.GetRequiredService<IFoo>(),
);
```

The returned lambda function always has the same signature: `Func<T, IServiceProvider, Task<object>>`, where `T` is the type of the function class (`MyFunction` in this case). For classes that have an `Invoke` method instead of `InvokeAsync`, the returned value is wrapped in a task using `Task.FromResult`.

This allows a very flexible API for FaaS functions, without having to stick to an arbitrary interface.

### Argument Resolution

Arguments to the `Invoke` or `InvokeAsync` method are resolved using an `IArgumentResolver`. The default implementation, `ArgumentResolver`, handles the arguments the following way:

- `string` arguments named `input` are treated as raw input, resolved using `IInput.AsString()`
- Object arguments named `input` as treated as JSON, resolved using `IInput.AsJson<T>()`
- `IServiceProvider` is passed through as-is
- Other interfaces are resolved through the dependency injection container, using `services.GetRequiredService<T>()`

## Usage

When configuring your `IServiceCollection`, call `services.AddFaasUtils()` (in the `FaasUtils.Extensions` namespace) to add the required services:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddFaasUtils();
}
```

To customize the `IArgumentResolver` used, use `services.replace`:

```csharp
services.Replace(ServiceDescriptor.Transient<IArgumentResolver, MyCustomArgumentResolver>();
```
