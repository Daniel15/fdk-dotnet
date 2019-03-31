using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FnProject.Fdk.Result
{
	/// <summary>
	/// Returns the content of a stream as a result.
	/// </summary>
	public class StreamResult : FnResult
	{
		private readonly Stream _stream;

		public StreamResult(Stream stream)
		{
			_stream = stream;
		}

		/// <summary>
		/// Writes the result body to the output stream
		/// </summary>
		protected override async Task WriteResultBody(HttpResponse response)
		{
			await _stream.CopyToAsync(response.Body);
			_stream.Dispose();
		}
	}
}
