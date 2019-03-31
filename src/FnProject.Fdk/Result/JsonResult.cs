using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace FnProject.Fdk.Result
{
	/// <summary>
	/// Returns an object serialized as JSON
	/// </summary>
	public class JsonResult : FnResult
	{
		private readonly object _result;

		/// <summary>
		/// Gets or sets the settings used for JSON serialization
		/// </summary>
		public JsonSerializerSettings JsonSerializerSettings { get; set; }

		public JsonResult(object result)
		{
			_result = result;
			ContentType = "application/json";
		}

		/// <summary>
		/// Writes the result body to the output stream
		/// </summary>
		protected override async Task WriteResultBody(HttpResponse response)
		{
			using (var writer = new HttpResponseStreamWriter(response.Body, Encoding))
			{
				var jsonSerializer = JsonSerializer.Create(JsonSerializerSettings);
				jsonSerializer.Serialize(writer, _result);
				await writer.FlushAsync();
			}
		}
	}
}
