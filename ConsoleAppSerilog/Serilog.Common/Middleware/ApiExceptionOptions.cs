using System;
using Microsoft.AspNetCore.Http;

namespace Serilog.Common.Middleware
{
    public class ApiExceptionOptions
    {       
        public Action<HttpContext, Exception, ApiError> AddResponseDetails { get; set; }        
    }
}
