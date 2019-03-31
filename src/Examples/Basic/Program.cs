using FnProject.Fdk;

namespace FnProject.Examples.Basic
{
	class Program
	{
		static void Main(string[] args)
		{
			FdkHandler.Handle(async (ctx, input) =>
			{
				// Plain text
				// return "Hello world!";

				// Objects are automatically serialized as JSON
				return new { message = "Hello world!" };

				// Streams can be returned. They will be automatically closed when the
				// request completes
				// return File.Open("c:\\temp\\test.txt", FileMode.Open, FileAccess.Read);

				// For more complex scenarios, you can create result instances directly
			});
		}
	}
}
