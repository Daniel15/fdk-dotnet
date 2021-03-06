﻿namespace FnProject.Examples.Basic
{
	public class HelloWorldFunction
	{
		public string Invoke(string input)
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
