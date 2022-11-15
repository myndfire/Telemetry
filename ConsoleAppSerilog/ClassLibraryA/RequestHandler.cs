using Microsoft.Extensions.Logging;

namespace ClassLibraryA
{
    public class RequestHandler
    {
        private ILogger<RequestManager> _logger;
        public RequestHandler(ILogger<RequestManager> logger)
        {
            _logger = logger;
        }
        public void ProcessRequest()
        {
            _logger.LogTrace("RequestHandler (Trace): RequestHandler Log..");
            _logger.LogDebug("RequestHandler (Debug): RequestHandler Log..");
            _logger.LogInformation("RequestHandler (Information): RequestHandler Log..");
            _logger.LogWarning("RequestHandler (Warning): RequestHandler Log..");
            _logger.LogError("RequestHandler (Error): RequestHandler Log..");
            _logger.LogCritical("RequestHandler (Fatal): RequestHandler Log..");
        }
    }
}