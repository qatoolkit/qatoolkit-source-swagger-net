using QAToolKit.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace QAToolKit.Source.Swagger
{
    /// <summary>
    /// Swagger request filtering
    /// </summary>
    public class SwaggerRequestFilter
    {
        private readonly IList<HttpRequest> _requests;

        /// <summary>
        /// Swagger Request filter
        /// </summary>
        /// <param name="requests"></param>
        public SwaggerRequestFilter(IList<HttpRequest> requests)
        {
            _requests = requests;
        }
        /// <summary>
        /// Filter out the requests by the specified filters
        /// </summary>
        /// <param name="requestFilter"></param>
        /// <returns></returns>
        public IList<HttpRequest> FilterRequests(RequestFilter requestFilter)
        {
            var requestsLocal = new List<HttpRequest>();

            if (requestFilter.AuthenticationTypes != null)
            {
                requestsLocal.AddRange(_requests.Where(request => requestFilter.AuthenticationTypes.ToList().Any(x => request.AuthenticationTypes.Contains(x))));
            }

            if (requestFilter.TestTypes != null)
            {
                requestsLocal.AddRange(_requests.Where(request => requestFilter.TestTypes.ToList().Any(x => request.TestTypes.Contains(x))));
            }

            if (requestFilter.EndpointNameWhitelist != null)
            {
                requestsLocal.AddRange(_requests.Where(request => requestFilter.EndpointNameWhitelist.Any(x => x == request.OperationId)));
            }

            return requestsLocal.ToList();
        }
    }
}
