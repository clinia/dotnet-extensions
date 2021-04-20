using System;

namespace Clinia.Logging.AspnetCore
{
    public class RequestLoggingOptions
    {
        public RequestLoggingOptions()
        {
            TraceEndpointNames = Array.Empty<string>();
        }

        public string[] TraceEndpointNames { get; set; }
    }
}