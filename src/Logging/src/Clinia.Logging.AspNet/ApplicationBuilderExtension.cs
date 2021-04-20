using Clinia.Logging.AspnetCore;
using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Events;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder UseCliniaRequestLogging(this IApplicationBuilder app)
        {
            return app.UseSerilogRequestLogging(opts =>
            {
                opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest;
                opts.GetLevel = LogHelper.GetRequestLevel(LogEventLevel.Verbose, "Health checks");
            });
        }
    }
}