using FnProject.Fdk;
using System.Threading.Tasks;

namespace FnProject.Examples.Basic
{
	class Program
	{
		static void Main(string[] args)
		{
			FdkHandler.Handle(async (ctx, input) =>
			{
				return "Hello world!";
			});
		}
	}
}
