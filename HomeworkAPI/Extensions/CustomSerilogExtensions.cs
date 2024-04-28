using HomeworkApi.Domain.Logging;
using Serilog;
using Serilog.Events;

namespace HomeworkAPI.Extensions
{
    public static class CustomSerilogExtensions
    {
        public static ILoggingBuilder AddCustomSerilog(this ILoggingBuilder logging, IConfiguration configuration)
        {
            // Add Serilog
            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()

                // Log everything to console
                .WriteTo.Console()

                // Log to file: Only requests tagged with special context info
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(evt => evt.Properties.ContainsKey(nameof(LogTarget.ForceToDisk)))
                    .WriteTo.File("Logs/log.txt", LogEventLevel.Warning, rollingInterval: RollingInterval.Hour))

                .CreateLogger();

            // Microsoft Extensions logger
            logging.AddSerilog(logger);

            // Global Serilog logger
            Log.Logger = logger;

            return logging;
        }
    }
}
