using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace FaasUtils.Tests.Utils
{
	/// <summary>
	/// Utilities for <see cref="Input"/>s in unit tests.
	/// </summary>
	public static class InputTestUtils
	{
		/// <summary>
		/// Creates a new <see cref="Input"/> from the given string
		/// </summary>
		public static Input CreateTestInput(string input)
		{
			var httpContext = Substitute.For<HttpContext>();
			var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
			httpContextAccessor.HttpContext.Returns(httpContext);

			var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(input));
			httpContext.Request.Body.Returns(bodyStream);
			return new Input(httpContextAccessor);
		}
	}
}
