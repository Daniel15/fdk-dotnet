using System.IO;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FaasUtils
{
	/// <summary>
	/// Represents the input to a function
	/// </summary>
	public class Input : IInput
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public Input(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		/// <summary>
		/// Gets the input as a string.
		/// </summary>
		public string AsString()
		{
			using (var reader = new StreamReader(_httpContextAccessor.HttpContext.Request.Body))
			{
				return reader.ReadToEnd();
			}
		}

		/// <summary>
		/// Gets the input as a dynamic JSON object.
		/// </summary>
		/// <returns></returns>
		public dynamic AsJson()
		{
			return JObject.Parse(AsString());
		}

		/// <summary>
		/// Gets the input as a dynamic JSON object.
		/// </summary>
		/// <returns></returns>
		public T AsJson<T>()
		{
			return JsonConvert.DeserializeObject<T>(AsString());
		}

		/// <summary>
		/// Gets the input as a stream.
		/// </summary>
		public Stream AsStream()
		{
			return _httpContextAccessor.HttpContext.Request.Body;
		}
	}
}
