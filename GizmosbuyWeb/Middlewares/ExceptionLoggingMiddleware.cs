using log4net;

namespace Gizmosbuy.Web.Middlewares
{
    public class ExceptionLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ExceptionLoggingMiddleware));

        public ExceptionLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // Proceed to the next middleware
            }
            catch (Exception ex)
            {
                LogException(ex);
                throw; 
            }
        }

        private void LogException(Exception ex)
        {
            var logEntry = Environment.NewLine + $"""
            ---------------------------------------------
            Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
            Message: {ex.Message}
            Source: {ex.Source}
            StackTrace: {ex.StackTrace}
            ---------------------------------------------
            """;

            _logger.Error(logEntry);
        }
    }

}
