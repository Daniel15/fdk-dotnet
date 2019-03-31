using System.Threading.Tasks;

namespace FnProject.Fdk
{
	/// <summary>
	/// Represents a Fn function that can be invoked
	/// </summary>
	public interface IFunction
	{
		/// <summary>
		/// Invokes the specified function
		/// </summary>
		/// <param name="ctx">Execution context for the function</param>
		/// <param name="input">Input to the function</param>
		/// <returns>Data to return from the request</returns>
		Task<object> InvokeAsync(IContext ctx, IInput input);
	}
}
