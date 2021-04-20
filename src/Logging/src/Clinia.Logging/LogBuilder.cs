using Elastic.Apm.SerilogEnricher;
using Elastic.CommonSchema.Serilog;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace Clinia.Logging
{
    public static class LogBuilder
    {
        public static LoggerConfiguration CreateDefaultLoggingConfiguration(bool useEcs = true)
        {
            var configuration = new LoggerConfiguration();

            if (useEcs)
            {
                configuration
                    .WriteTo.Console(new EcsTextFormatter());
            }
            else
            {
                configuration
                    .WriteTo.Console(theme: AnsiConsoleTheme.Code);
            }

            configuration
                .Enrich.FromLogContext()
                .Enrich.WithElasticApmCorrelationInfo();

            return configuration;
        }
    }
}