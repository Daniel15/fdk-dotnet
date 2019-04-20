using System.Threading;
using System.Threading.Tasks;
using FaasUtils;
using FaasUtils.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using Xunit;

namespace FnProject.Fdk.Tests
{
	public class FunctionExpressionTreeBuilderInputTest
	{
		private class FunctionWithCancellationToken
		{
			public Task<object> InvokeAsync(CancellationToken timedOut)
			{
				Assert.True(timedOut.CanBeCanceled);
				return Task.FromResult<object>(null);
			}
		}

		[Fact]
		public async Task TestPassesCancellationToken()
		{
			var tokenSource = new CancellationTokenSource();
			var context = Substitute.For<IContext>();
			context.TimedOut.Returns(tokenSource.Token);

			var services = new ServiceCollection()
				.AddFaasUtils()
				.Replace(ServiceDescriptor.Transient<IArgumentResolver, FnArgumentResolver>())
				.AddScoped<IContext>(_ => context)
				.BuildServiceProvider();

			var function = services.GetRequiredService<IFunctionExpressionTreeBuilder>()
				.CreateLambda<FunctionWithCancellationToken>();
			await function(
				new FunctionWithCancellationToken(),
				services
			);
		}
	}
}
