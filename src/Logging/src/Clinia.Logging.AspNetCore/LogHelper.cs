using System;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;

namespace Clinia.Logging.AspnetCore
{
    public static class LogHelper
    {
        public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            var request = httpContext.Request;
            diagnosticContext.Set("Host", request.Host);
            diagnosticContext.Set("Protocol", request.Protocol);
            diagnosticContext.Set("Scheme", request.Scheme);
            if (request.QueryString.HasValue)
            {
                diagnosticContext.Set("QueryString", request.QueryString.Value);
            }

            diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

            var endpoint = httpContext.GetEndpoint();
            if (endpoint is object)
            {
                diagnosticContext.Set("EndpointName", endpoint.DisplayName);
            }
        }

        public static Func<HttpContext, double, Exception, LogEventLevel> GetRequestLevel(
            LogEventLevel traceLevel,
            params string[] traceEndpointNames)
        {
            if (traceEndpointNames is null || traceEndpointNames.Length == 0)
            {
                throw new ArgumentNullException(nameof(traceEndpointNames));
            }

            return (ctx, _, ex) =>
            {
                if (IsError(ctx, ex))
                {
                    return LogEventLevel.Error;
                }

                if (IsTraceEndpoint(ctx, traceEndpointNames))
                {
                    return traceLevel;
                }

                return LogEventLevel.Information;
            };
        }
        
        private static bool IsError(HttpContext ctx, Exception ex) => ex != null || ctx.Response.StatusCode > 499;

        private static bool IsTraceEndpoint(HttpContext httpContext, string[] traceEndpoints)
        {
            var endpoint = httpContext.GetEndpoint();

            if (endpoint is object)
            {
                for (var i = 0; i < traceEndpoints.Length; i++)
                {
                    if (string.Equals(traceEndpoints[i], endpoint.DisplayName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}