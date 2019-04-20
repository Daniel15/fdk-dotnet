using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using NSubstitute;
using Xunit;

namespace FnProject.Fdk.Tests
{
	public class FnArgumentResolverTest
	{
		[Fact]
		public void TestResolvesCancellationToken()
		{
			var result = Resolve(typeof(CancellationToken));
			Assert.Equal(
				"Convert(services.GetRequiredService(FnProject.Fdk.IContext), IContext).TimedOut",
				result.ToString()
			);
		}

		private Expression Resolve(Type type)
		{
			var param = Substitute.For<ParameterInfo>();
			param.ParameterType.Returns(type);
			return Resolve(param);
		}

		private Expression Resolve(ParameterInfo param)
		{
			var servicesArg = Expression.Parameter(typeof(IServiceProvider), "services");
			var resolver = new FnArgumentResolver();
			return resolver.Resolve(param, servicesArg);
		}
	}
}
