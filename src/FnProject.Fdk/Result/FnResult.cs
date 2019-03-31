using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

namespace FnProject.Fdk.Result
{
	/// <summary>
	/// Represents a function's result.
	/// </summary>
	public abstract class FnResult
	{
		/// <summary>
		/// Gets or sets the Content-Type of the response.
		/// </summary>
		public string ContentType { get; set; } = "text/plain";

		/// <summary>
		/// Gets or sets the encoding to use for the response
		/// </summary>
		public Encoding Encoding { get; set; } = Encoding.UTF8;

		/// <summary>
		/// Gets or sets custom HTTP headers to include with the response
		/// </summary>
		public IHeaderDictionary Headers { get; } = new HeaderDictionary();

		/// <summary>
		/// Gets or sets the HTTP status code
		/// </summary>
		public int HttpStatus { get; set; } = StatusCodes.Status200OK;

		/// <summary>
		/// Writes the result (including headers) to the output stream.
		/// </summary>
		public async Task WriteResult(HttpResponse response)
		{
			response.ContentType = ContentType;
			if (HttpStatus != StatusCodes.Status200OK)
			{
				response.Headers["Fn-Http-Status"] = HttpStatus.ToString();
			}
			foreach (var header in Headers)
			{
				response.Headers["Fn-Http-H-" + header.Key] = header.Value;
			}

			await WriteResultBody(response);
		}

		/// <summary>
		/// Writes the result body to the output stream
		/// </summary>
		protected abstract Task WriteResultBody(HttpResponse response);
	}
}
