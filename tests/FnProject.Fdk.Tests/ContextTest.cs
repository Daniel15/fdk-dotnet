using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Xunit;

namespace FnProject.Fdk.Tests
{
	public class ContextTest
	{
		[Fact]
		public void TestGetsHeaders()
		{
			var inputHeaders = new HeaderDictionary
			{
				{"Fn-Http-H-Accept", "text/html"},
				{"Fn-Http-H-User-Agent", "Mozilla/5.0"},
				{"Fn-Http-H-X-Custom", "foo"},
				{"User-Agent", "Go-http-client/1.1"},
			};
			var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
			httpContextAccessor.HttpContext.Request.Headers.Returns(inputHeaders);

			var ctx = new Context(Substitute.For<IConfig>(), httpContextAccessor);

			var fnHeaders = ctx.Headers;
			Assert.Equal(3, fnHeaders.Count);
			Assert.Equal("text/html", fnHeaders["Accept"]);
			Assert.Equal("Mozilla/5.0", fnHeaders["User-Agent"]);
			Assert.Equal("foo", fnHeaders["X-Custom"]);
		}
	}
}
