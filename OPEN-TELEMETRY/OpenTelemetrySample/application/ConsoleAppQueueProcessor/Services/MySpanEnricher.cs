using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace ConsoleAppQueueProcessor.Services
{
    public class MySpanEnricher
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MySpanEnricher(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Enrich()
        {
            var context = _httpContextAccessor.HttpContext;
            var activity = context.Features.Get<IHttpActivityFeature>()?.Activity;
            activity?.SetTag("ConsoleAppQueueProcessor", "online");
        }
    }
}
