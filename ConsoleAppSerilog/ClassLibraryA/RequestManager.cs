using System;
using Microsoft.Extensions.Logging;

namespace ClassLibraryA
{
    public class RequestManager
    {
        private ILogger<RequestManager> _logger;
        private RequestHandler _requestHandler;
        public RequestManager(ILogger<RequestManager> logger, RequestHandler requestHandler)
        {
            _logger = logger;
            _requestHandler = requestHandler;
        }
        public void StartProcessing()
        {
            //Surround by scope so that all the logs within the using statement gets tagged with the Scope property set to "ClassARun()Request"
            using (_logger.BeginScope("ClassARun()Request"))
            {
                _logger.LogTrace("RequestManager (Trace): RequestManager Log..");
                _logger.LogDebug("RequestManager (Debug): RequestManager Log..");
                _logger.LogInformation("RequestManager (Information): RequestManager Log..");
                _logger.LogWarning("RequestManager (Warning): RequestManager Log..");
                _logger.LogError("RequestManager (Error): RequestManager Log..");
                _logger.LogCritical("RequestManager (Fatal): RequestManager Log..");

                _requestHandler.ProcessRequest();
            }

        }
    }
}
