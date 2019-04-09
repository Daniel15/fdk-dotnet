using System.Runtime.InteropServices;
using FnProject.Fdk.Exceptions;
using NSubstitute;
using Xunit;

namespace FnProject.Fdk.Tests
{
	public class ConfigValidatorTest
	{
		[Fact]
		public void TestValidConfig()
		{
			var config = CreateConfig();
			ConfigValidator.Validate(config);
		}

		[Fact]
		public void TestInvalidFormat()
		{
			var config = CreateConfig();
			config.Format.Returns("invalid");
			var exception = Assert.Throws<InvalidConfigException>(() => ConfigValidator.Validate(config));
			Assert.Equal("http-stream is the only supported format", exception.Message);
		}

		[Fact]
		public void TestUnknownSocketType()
		{
			var config = CreateConfig();
			config.ListenerSocketType.Returns(SocketType.Unknown);
			config.Listener.Returns("foobar");
			var exception = Assert.Throws<InvalidConfigException>(() => ConfigValidator.Validate(config));
			Assert.Equal("Invalid listener: foobar", exception.Message);
		}

		[SkippableFact]
		public void TestDoesNotAllowUnixSocketOnWindows()
		{
			Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));

			var config = CreateConfig();
			config.ListenerSocketType.Returns(SocketType.Unix);
			var exception = Assert.Throws<InvalidConfigException>(() => ConfigValidator.Validate(config));
			Assert.Equal("UNIX sockets are not supported on Windows", exception.Message);
		}

		private IConfig CreateConfig()
		{
			var config = Substitute.For<IConfig>();
			config.Format.Returns("http-stream");
			config.ListenerSocketType.Returns(SocketType.Tcp);
			return config;
		}
	}
}
