using DataConsulting.PuntoVentaComercial.API.Middleware;

namespace DataConsulting.PuntoVentaComercial.API.Extensions
{
    internal static class ApplicationBuilderExtensions
    {
        public static void UseCustomExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionHandler>();
        }

        internal static IApplicationBuilder UseLogContextTraceLogging(this IApplicationBuilder app)
        {
            app.UseMiddleware<LogContextTraceLoggingMiddleware>();

            return app;
        }
    }
}
