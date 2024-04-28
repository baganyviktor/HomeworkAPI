using HomeworkApi.Domain.Logging;
using System.Diagnostics;

namespace HomeworkAPI.Request.Middleware
{
    public class RequestLoggerMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public static readonly string _logMessage = "Request {Method} {Path} took {ElapsedSeconds} seconds";

        public async Task InvokeAsync(HttpContext context, ILogger<RequestLoggerMiddleware> logger)
        {
            var stopWatch = Stopwatch.StartNew();

            // Continue executing
            await _next(context);

            stopWatch.Stop();

            ArgumentException.ThrowIfNullOrWhiteSpace(_logMessage);

            var elapsedTime = stopWatch.Elapsed;
            var slowRequest = elapsedTime.TotalSeconds > 3;

            var logParams = new object[]
            {
                context.Request.Method,
                context.Request.Path,
                elapsedTime.TotalSeconds
            };

            if (!slowRequest)
            {
                logger.LogInformation(_logMessage, logParams);
            }
            else
            {
                var loggerContext = new Dictionary<string, object>
                {
                    [nameof(LogTarget.ForceToDisk)] = ""
                };

                using (logger.BeginScope(loggerContext))
                {
                    logger.LogWarning(_logMessage, logParams);
                }
            }
        }
    }
}

