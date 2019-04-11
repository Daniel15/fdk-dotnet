using System;

namespace FnProject.Fdk.Exceptions
{
	public class InvalidFunctionException : Exception
	{
		public InvalidFunctionException(string message) : base(message) { }
	}
}
