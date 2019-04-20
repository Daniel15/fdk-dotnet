using System.Threading.Tasks;
using FaasUtils.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FaasUtils.Tests
{
	public class FunctionExpressionTreeBuilderOutputTest
	{
		private class FunctionReturningTaskOfString
		{
			public Task<string> InvokeAsync()
			{
				return Task.FromResult("Hello World!");
			}
		}
		[Fact]
		public async Task TestCanReturnTaskOfString()
		{
			var services = new ServiceCollection()
				.AddFaasUtils()
				.BuildServiceProvider();

			var function = services.GetRequiredService<IFunctionExpressionTreeBuilder>()
				.CreateLambda<FunctionReturningTaskOfString>();
			var result = await function(new FunctionReturningTaskOfString(), services);
			Assert.Equal("Hello World!", result);
		}

		private class FunctionReturningString
		{
			public string Invoke()
			{
				return "Hello Non-Async World!";
			}
		}
		[Fact]
		public async Task TestCanReturnStringSync()
		{
			var services = new ServiceCollection()
				.AddFaasUtils()
				.BuildServiceProvider();

			var function = services.GetRequiredService<IFunctionExpressionTreeBuilder>()
				.CreateLambda<FunctionReturningString>();
			var result = await function(new FunctionReturningString(), services);
			Assert.Equal("Hello Non-Async World!", result);
		}
	}
}
