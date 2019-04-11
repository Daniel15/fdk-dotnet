using FnProject.Fdk;

namespace FnProject.Examples.Basic
{
	class Program
	{
		static void Main(string[] args)
		{
			// Example of an inline function
			//BasicExample();

			// Example of a function in a separate class
			FdkHandler.Handle<HelloWorldFunction>();
		}

		static void BasicExample()
		{
			FdkHandler.Handle((ctx, input) =>
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

				// When performing async operations, use ctx.TimedOut as the cancellation token
				// It will be triggered if the function times out.
				/*while (true)
				{
					await Task.Delay(1000, ctx.TimedOut);
				}*/
			});
		}
	}
}
