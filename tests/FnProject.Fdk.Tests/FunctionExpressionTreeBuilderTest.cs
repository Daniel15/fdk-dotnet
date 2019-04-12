using System.Threading.Tasks;
using FnProject.Fdk.Exceptions;
using FnProject.Fdk.Tests.Utils;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace FnProject.Fdk.Tests
{
	public class FunctionExpressionTreeBuilderTest
	{
		private class FunctionWithServiceInInvoke
		{
			public Task<object> InvokeAsync(IFoo foo)
			{
				Assert.IsType<Foo>(foo);
				return Task.FromResult<object>(null);
			}
		}
		[Fact]
		public async Task TestResolvesServicesOnInvoke()
		{
			var services = new ServiceCollection()
				.AddTransient<IFoo, Foo>()
				.BuildServiceProvider();

			var function = FunctionExpressionTreeBuilder.CreateLambda<FunctionWithServiceInInvoke>();
			await function(
				new FunctionWithServiceInInvoke(),
				services
			);
		}

		[Fact]
		public void TestThrowsIfNoInvokeOrInvokeAsyncMethod()
		{
			var ex = Assert.Throws<InvalidFunctionException>(
				() => FunctionExpressionTreeBuilder.CreateLambda<Foo>()
			);
			Assert.Equal("Foo has no InvokeAsync or Invoke method.", ex.Message);
		}

		private interface IFoo { }
		private class Foo : IFoo { }
	}
}
