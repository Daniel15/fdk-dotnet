using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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
		/// Writes the result (including headers) to the output stream.
		/// </summary>
		public async Task WriteResult(HttpResponse response)
		{
			response.ContentType = ContentType;
			await WriteResultBody(response);
		}

		/// <summary>
		/// Writes the result body to the output stream
		/// </summary>
		protected abstract Task WriteResultBody(HttpResponse response);
	}
}
