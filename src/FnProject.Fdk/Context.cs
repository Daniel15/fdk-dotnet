using Microsoft.AspNetCore.Http;

namespace FnProject.Fdk
{
	/// <summary>
	/// Represents the context that a function is executing in
	/// </summary>
	public class Context : IContext
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		
		public IConfig Config { get; }

		public Context(IConfig config, IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
			Config = config;
		}
	}
}
