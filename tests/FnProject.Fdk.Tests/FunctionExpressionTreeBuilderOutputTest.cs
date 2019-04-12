using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FnProject.Fdk.Tests
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
				.BuildServiceProvider();

			var function = FunctionExpressionTreeBuilder.CreateLambda<FunctionReturningTaskOfString>();
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
				.BuildServiceProvider();

			var function = FunctionExpressionTreeBuilder.CreateLambda<FunctionReturningString>();
			var result = await function(new FunctionReturningString(), services);
			Assert.Equal("Hello Non-Async World!", result);
		}
	}
}
