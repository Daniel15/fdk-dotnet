using System.Threading.Tasks;

namespace FnProject.Examples.Basic
{
	public class HelloWorldFunction
	{
		public async Task<object> InvokeAsync(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				input = "world";
			}

			// Plain text
			return $"Hello {input}!";

			// Objects are automatically serialized as JSON
			// return new { message = "Hello " + inputStr + "!" };
		}
	}
}
