using QAToolKit.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace QAToolKit.Source.Swagger
{
    /// <summary>
    /// Swagger request filtering
    /// </summary>
    public static class SwaggerRequestFilter
    {
        /// <summary>
        /// Filter out the requests by the specified filters
        /// </summary>
        /// <param name="requests"></param>
        /// <param name="requestFilter"></param>
        /// <returns></returns>
        public static IList<HttpTestRequest> FilterRequests(IList<HttpTestRequest> requests, RequestFilter requestFilter)
        {
            var requestsLocal = new List<HttpTestRequest>();

            if (requestFilter.AuthenticationTypes != null)
            {
                requestsLocal.AddRange(requests.Where(request => requestFilter.AuthenticationTypes.ToList().Any(x => x == request.AuthenticationTypes)));
            }

            if (requestFilter.TestTypes != null)
            {
                requestsLocal.AddRange(requests.Where(request => requestFilter.TestTypes.ToList().Any(x => x == request.TestTypes)));
            }

            if (requestFilter.EndpointNameWhitelist != null)
            {
                requestsLocal.AddRange(requests.Where(request => requestFilter.EndpointNameWhitelist.Any(x => x == request.OperationId)));
            }

            return requestsLocal.ToList();
        }
    }
}
