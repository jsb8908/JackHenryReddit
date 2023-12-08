using JackHenryReddit.Common;

namespace JackHenryReddit.Middleware
{
    public class JackHenryExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JackHenryExceptionHandler> _logger;
        // add 3rd party logging here (Application Insights, etc...)

        public JackHenryExceptionHandler(RequestDelegate next, ILogger<JackHenryExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                //log anything of interest before....

                // Call Endpoint and catch any exceptions globally. 
                await _next(httpContext);

                //... and after invoking the endpoint
            }
            catch (Exception ex)
            {
                _logger.LogExceptionJackHenryReddit(ex);
            }
        }
    }
}
