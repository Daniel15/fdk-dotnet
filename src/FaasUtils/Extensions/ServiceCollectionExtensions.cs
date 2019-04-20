using Microsoft.Extensions.DependencyInjection;

namespace FaasUtils.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddFaasUtils(this IServiceCollection services)
		{
			// Generally only used once at app startup, so make it transient instead of singleton
			services.AddTransient<IFunctionExpressionTreeBuilder, FunctionExpressionTreeBuilder>();
			services.AddTransient<IArgumentResolver, ArgumentResolver>();

			services.AddScoped<IInput, Input>();
			return services;
		}
	}
}
