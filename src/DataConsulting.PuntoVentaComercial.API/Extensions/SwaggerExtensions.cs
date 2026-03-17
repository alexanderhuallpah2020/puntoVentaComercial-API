using Microsoft.OpenApi;

namespace DataConsulting.PuntoVentaComercial.API.Extensions
{
    internal static class SwaggerExtensions
    {
        internal static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "PuntoVentaComercial API",
                    Version = "v1",
                    Description = "PuntoVentaComercial API built using clean architecture."
                });

                options.CustomSchemaIds(t => t.FullName?.Replace("+", "."));
            });

            return services;
        }
    }
}
