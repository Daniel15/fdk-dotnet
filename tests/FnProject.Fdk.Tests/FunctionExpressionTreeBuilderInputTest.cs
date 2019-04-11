using System.Threading.Tasks;
using FnProject.Fdk.Tests.Utils;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace FnProject.Fdk.Tests
{
	public class FunctionExpressionTreeBuilderInputTest
	{
		private class FunctionWithStringInput
		{
			public async Task<object> InvokeAsync(string input)
			{
				return "Hello " + input;
			}
		}
		[Fact]
		public async Task TestPassesInputAsString()
		{
			var function = FunctionExpressionTreeBuilder.CreateLambda<FunctionWithStringInput>();
			var result = await function(
				new FunctionWithStringInput(),
				Substitute.For<IContext>(),
				InputTestUtils.CreateTestInput("Daniel"),
				new ServiceCollection().BuildServiceProvider()
			);

			Assert.Equal("Hello Daniel", result);
		}

		private class NameInput
		{
			public string Name { get; set; }
		}
		private class FunctionWithJsonInput
		{
			public async Task<object> InvokeAsync(NameInput input)
			{
				return "Hello " + input.Name;
			}
		}
		[Fact]
		public async Task TestPassesInputAsJson()
		{
			var function = FunctionExpressionTreeBuilder.CreateLambda<FunctionWithJsonInput>();
			var result = await function(
				new FunctionWithJsonInput(),
				Substitute.For<IContext>(),
				InputTestUtils.CreateTestInput(@"{""Name"": ""Daniel""}"),
				new ServiceCollection().BuildServiceProvider()
			);

			Assert.Equal("Hello Daniel", result);
		}

		private class FunctionWithDynamicJsonInput
		{
			public async Task<object> InvokeAsync(dynamic input)
			{
				return "Hello " + input.Name;
			}
		}
		[Fact]
		public async Task TestPassesInputAsDynamicJson()
		{
			var function = FunctionExpressionTreeBuilder.CreateLambda<FunctionWithDynamicJsonInput>();
			var result = await function(
				new FunctionWithDynamicJsonInput(),
				Substitute.For<IContext>(),
				InputTestUtils.CreateTestInput(@"{""Name"": ""Daniel""}"),
				new ServiceCollection().BuildServiceProvider()
			);

			Assert.Equal("Hello Daniel", result);
		}
	}
}
