using FnProject.Fdk;
using FnProject.Fdk.Result;
using Microsoft.AspNetCore.Http;

namespace FnProject.Examples.Basic
{
	class Program
	{
		static void Main(string[] args)
		{
			FdkHandler.Handle(async (ctx, input) =>
			{

				var inputStr = input.AsString();
				if (string.IsNullOrWhiteSpace(inputStr))
				{
					inputStr = "world";
				}

				// Plain text
				return "Hello " + inputStr + "!";

				// Objects are automatically serialized as JSON
				// return new { message = "Hello " + inputStr + "!" };

				// Streams can be returned. They will be automatically closed when the
				// request completes
				// return File.Open("c:\\temp\\test.txt", FileMode.Open, FileAccess.Read);

				// For more complex scenarios, you can create result instances directly
				/*return new RawResult("Hello " + inputStr + "!")
				{
					// Example of custom status code
					HttpStatus = StatusCodes.Status202Accepted,
					// Example of custom headers
					Headers =
					{
						["X-Some-Header"] = "foo"
					}
				};*/
			});
		}
	}
}
