using System;

namespace FaasUtils.Exceptions
{
	public class InvalidFunctionException : Exception
	{
		public InvalidFunctionException(string message) : base(message) { }
	}
}
