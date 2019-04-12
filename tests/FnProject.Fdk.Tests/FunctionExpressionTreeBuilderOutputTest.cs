using System.Threading.Tasks;
using FnProject.Fdk.Tests.Utils;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
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
	}
}
