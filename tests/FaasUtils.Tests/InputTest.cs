using System.IO;
using FaasUtils.Tests.Utils;
using Xunit;

namespace FaasUtils.Tests
{
	public class InputTest
	{
		[Fact]
		public void TestHandlesString()
		{
			var rawInput = InputTestUtils.CreateTestInput("Hello World!");
			var input = rawInput.AsString();
			Assert.Equal("Hello World!", input);
		}

		[Fact]
		public void TestHandlesDynamicJson()
		{
			var rawInput = InputTestUtils.CreateTestInput(@"{""Message"": ""Hello World!""}");
			var input = rawInput.AsJson();
			Assert.Equal("Hello World!", (string)input.Message);
		}

		[Fact]
		public void TestHandlesStronglyTypedJson()
		{
			var rawInput = InputTestUtils.CreateTestInput(@"{""Message"": ""Hello World!""}");
			var input = rawInput.AsJson<HelloWorldPayload>();
			Assert.Equal("Hello World!", input.Message);
		}

		[Fact]
		public void TestHandlesStream()
		{
			var rawInput = InputTestUtils.CreateTestInput("Hello World!");
			var inputStream = rawInput.AsStream();

			using (var reader = new StreamReader(inputStream))
			{
				var input = reader.ReadToEnd();
				Assert.Equal("Hello World!", input);
			}
		}

		private class HelloWorldPayload
		{
			public string Message { get; set; }
		}
	}
}
