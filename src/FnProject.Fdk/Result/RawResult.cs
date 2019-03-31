using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FnProject.Fdk.Result
{
	/// <summary>
	/// Returns a raw string as a result.
	/// </summary>
	public class RawResult : FnResult
	{
		private readonly string _result;

		public RawResult(string result)
		{
			_result = result;
		}

		/// <summary>
		/// Writes the result to the output stream
		/// </summary>
		protected override async Task WriteResultBody(HttpResponse response)
		{
			await response.WriteAsync(_result, Encoding);
		}
	}
}
