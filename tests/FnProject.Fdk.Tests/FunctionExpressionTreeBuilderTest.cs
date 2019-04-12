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
				.AddScoped(_ => Substitute.For<IContext>())
				.AddScoped<IInput>(_ => InputTestUtils.CreateTestInput(string.Empty))
				.BuildServiceProvider();

			var function = FunctionExpressionTreeBuilder.CreateLambda<FunctionWithServiceInInvoke>();
			await function(
				new FunctionWithServiceInInvoke(),
				services
			);
		}

		[Fact]
		public void TestThrowsIfNoInvokeAsyncMethod()
		{
			var ex = Assert.Throws<InvalidFunctionException>(
				() => FunctionExpressionTreeBuilder.CreateLambda<Foo>()
			);
			Assert.Equal("Foo has no InvokeAsync method", ex.Message);
		}

		private interface IFoo { }
		private class Foo : IFoo { }
	}
}
