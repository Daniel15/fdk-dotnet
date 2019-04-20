using System.Threading.Tasks;
using FaasUtils.Exceptions;
using FaasUtils.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FaasUtils.Tests
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
				.AddFaasUtils()
				.AddTransient<IFoo, Foo>()
				.BuildServiceProvider();

			var function = services.GetRequiredService<IFunctionExpressionTreeBuilder>()
				.CreateLambda<FunctionWithServiceInInvoke>();
			await function(
				new FunctionWithServiceInInvoke(),
				services
			);
		}

		[Fact]
		public void TestThrowsIfNoInvokeOrInvokeAsyncMethod()
		{
			var services = new ServiceCollection()
				.AddFaasUtils()
				.BuildServiceProvider();

			var ex = Assert.Throws<InvalidFunctionException>(
				() => services.GetRequiredService<IFunctionExpressionTreeBuilder>().CreateLambda<Foo>()
			);
			Assert.Equal("Foo has no InvokeAsync or Invoke method.", ex.Message);
		}

		private interface IFoo { }
		private class Foo : IFoo { }
	}
}
