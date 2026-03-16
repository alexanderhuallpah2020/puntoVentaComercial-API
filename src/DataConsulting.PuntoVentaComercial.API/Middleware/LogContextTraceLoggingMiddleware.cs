using Serilog.Context;
using System.Diagnostics;

namespace DataConsulting.PuntoVentaComercial.API.Middleware
{
    internal sealed class LogContextTraceLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LogContextTraceLoggingMiddleware> _logger;

        public LogContextTraceLoggingMiddleware(
            RequestDelegate next,
            ILogger<LogContextTraceLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            string traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;

            using (LogContext.PushProperty("TraceId", traceId))
            {
                _logger.LogInformation("TraceId middleware activo. TraceId={TraceId}", traceId);
                await _next(context);
            }
        }
    }
}
