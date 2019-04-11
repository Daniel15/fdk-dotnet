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
				Substitute.For<IContext>(),
				InputTestUtils.CreateTestInput(string.Empty),
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

		private class FunctionWithStandardArgs
		{
			public Task<object> InvokeAsync(IContext ctx, IInput input)
			{
				Assert.NotNull(ctx);
				Assert.NotNull(input);
				return Task.FromResult<object>(null);
			}
		}
		[Fact]
		public async Task TestPassesStandardArgs()
		{
			var function = FunctionExpressionTreeBuilder.CreateLambda<FunctionWithStandardArgs>();
			await function(
				new FunctionWithStandardArgs(),
				Substitute.For<IContext>(),
				InputTestUtils.CreateTestInput(string.Empty),
				new ServiceCollection().BuildServiceProvider()
			);
		}

		private interface IFoo { }
		private class Foo : IFoo { }
	}
}
