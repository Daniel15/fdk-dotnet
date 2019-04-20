using System.Threading.Tasks;
using FaasUtils.Extensions;
using FaasUtils.Tests.Utils;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FaasUtils.Tests
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
			var services = new ServiceCollection()
				.AddFaasUtils()
				.AddScoped<IInput>(_ => InputTestUtils.CreateTestInput("Daniel"))
				.BuildServiceProvider();

			var function = services.GetRequiredService<IFunctionExpressionTreeBuilder>()
				.CreateLambda<FunctionWithStringInput>();
			var result = await function(
				new FunctionWithStringInput(),
				services
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
			var services = new ServiceCollection()
				.AddFaasUtils()
				.AddScoped<IInput>(_ => InputTestUtils.CreateTestInput(@"{""Name"": ""Daniel""}"))
				.BuildServiceProvider();
			var function = services.GetRequiredService<IFunctionExpressionTreeBuilder>()
				.CreateLambda<FunctionWithJsonInput>();
			var result = await function(
				new FunctionWithJsonInput(),
				services
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
			var services = new ServiceCollection()
				.AddFaasUtils()
				.AddScoped<IInput>(_ => InputTestUtils.CreateTestInput(@"{""Name"": ""Daniel""}"))
				.BuildServiceProvider();
			var function = services.GetRequiredService<IFunctionExpressionTreeBuilder>()
				.CreateLambda<FunctionWithDynamicJsonInput>();
			var result = await function(
				new FunctionWithDynamicJsonInput(),
				services
			);

			Assert.Equal("Hello Daniel", result);
		}
	}
}
