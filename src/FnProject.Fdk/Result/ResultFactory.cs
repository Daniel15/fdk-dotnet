using System.IO;

namespace FnProject.Fdk.Result
{
	/// <summary>
	/// Handles creating <see cref="FnResult"/>s based on the response content.
	/// </summary>
	public abstract class ResultFactory
	{
		/// <summary>
		/// Creates an <see cref="FnResult"/> based on the provided content.
		/// </summary>
		/// <param name="content">Content to create result for</param>
		/// <returns>Result</returns>
		public static FnResult Create(object content)
		{
			switch (content)
			{
				// Returned a result directly; just use it
				case FnResult result:
					return result;

				// String - Assume it's a raw response
				case string str:
					return new RawResult(str);

				// Stream - Return the contents of the stream
				case Stream stream:
					return new StreamResult(stream);

				// Anything else - Assume it's an object that needs to be JSON encoded
				default:
					return new JsonResult(content);
			}
		}
	}
}
