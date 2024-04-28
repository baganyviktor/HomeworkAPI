using HomeworkAPI.Request.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace HomeworkAPI.Test
{
    public class RequestLoggerTest
    {
        [Fact]
        public async void Test_Middleware_Information_Log()
        {
            var mockHttpContext = new DefaultHttpContext();
            var mockDelegate = new RequestDelegate(context => Task.CompletedTask);

            var middleware = new RequestLoggerMiddleware(mockDelegate);
            var mockLogger = new Mock<ILogger<RequestLoggerMiddleware>>();

            // Act
            await middleware.InvokeAsync(mockHttpContext, mockLogger.Object);

            // Verify method execution
            mockLogger.Verify(logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ), Times.Once);
        }

        [Fact]
        public async void Test_Middleware_Warning_Log()
        {
            var mockHttpContext = new DefaultHttpContext();
            RequestDelegate mockDelegate = (httpContext) =>
            {
                return Task.Delay(5000);
            };

            var middleware = new RequestLoggerMiddleware(mockDelegate);
            var mockLogger = new Mock<ILogger<RequestLoggerMiddleware>>();

            // Act
            await middleware.InvokeAsync(mockHttpContext, mockLogger.Object);

            // Verify method execution
            mockLogger.Verify(logger => logger.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ), Times.Once);
        }
    }
}