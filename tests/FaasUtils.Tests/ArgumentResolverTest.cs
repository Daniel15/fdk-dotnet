using System;
using System.Linq.Expressions;
using System.Reflection;
using NSubstitute;
using Xunit;

namespace FaasUtils.Tests
{
	public class ArgumentResolverTest
	{
		[Fact]
		public void TestResolvesServiceProvider()
		{
			var result = Resolve(typeof(IServiceProvider));
			Assert.Equal("services", result.ToString());
		}

		[Fact]
		public void TestResolvesString()
		{
			var result = Resolve(typeof(string));
			Assert.Equal(
				"Convert(services.GetRequiredService(FaasUtils.IInput), IInput).AsString()",
				result.ToString()
			);
		}

		[Fact]
		public void TestResolvesInterface()
		{
			var result = Resolve(typeof(IFoo));
			Assert.Equal(
				"Convert(services.GetRequiredService(FaasUtils.Tests.ArgumentResolverTest+IFoo), IFoo)",
				result.ToString()
			);
		}

		[Fact]
		public void TestResolvesJson()
		{
			var param = Substitute.For<ParameterInfo>();
			param.ParameterType.Returns(typeof(FooInput));
			param.Name.Returns("input");
			var result = Resolve(param);

			Assert.Equal(
				"Convert(services.GetRequiredService(FaasUtils.IInput), IInput).AsJson()",
				result.ToString()
			);
		}

		[Fact]
		public void TestReturnsVoidForRandomClass()
		{
			var param = Substitute.For<ParameterInfo>();
			// Class param NOT called "input"
			param.ParameterType.Returns(typeof(FooInput));
			param.Name.Returns("SomeRandomThing");
			var result = Resolve(param);

			Assert.Null(result);
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
			var resolver = new ArgumentResolver();
			return resolver.Resolve(param, servicesArg);
		}

		private interface IFoo { }
		private class FooInput { }
	}
}
