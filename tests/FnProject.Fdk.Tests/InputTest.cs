using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Xunit;

namespace FnProject.Fdk.Tests
{
	public class InputTest
	{
		[Fact]
		public void TestHandlesString()
		{
			var rawInput = CreateTestInput("Hello World!");
			var input = rawInput.AsString();
			Assert.Equal("Hello World!", input);
		}

		[Fact]
		public void TestHandlesDynamicJson()
		{
			var rawInput = CreateTestInput(@"{""Message"": ""Hello World!""}");
			var input = rawInput.AsJson();
			Assert.Equal("Hello World!", (string)input.Message);
		}

		[Fact]
		public void TestHandlesStronglyTypedJson()
		{
			var rawInput = CreateTestInput(@"{""Message"": ""Hello World!""}");
			var input = rawInput.AsJson<HelloWorldPayload>();
			Assert.Equal("Hello World!", input.Message);
		}

		[Fact]
		public void TestHandlesStream()
		{
			var rawInput = CreateTestInput("Hello World!");
			var inputStream = rawInput.AsStream();

			using (var reader = new StreamReader(inputStream))
			{
				var input = reader.ReadToEnd();
				Assert.Equal("Hello World!", input);
			}
		}

		private Input CreateTestInput(string input)
		{
			var httpContext = Substitute.For<HttpContext>();
			var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
			httpContextAccessor.HttpContext.Returns(httpContext);

			var bodyStream = new MemoryStream(Encoding.UTF8.GetBytes(input));
			httpContext.Request.Body.Returns(bodyStream);
			return new Input(httpContextAccessor);
		}

		private class HelloWorldPayload
		{
			public string Message { get; set; }
		}
	}
}
