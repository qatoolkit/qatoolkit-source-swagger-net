using QAToolKit.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QAToolKit.Source.Swagger
{
    /// <summary>
    /// Swagger request filtering by OperationId, TestType or AuthenticationType
    /// </summary>
    public class SwaggerRequestFilter
    {
        private readonly IEnumerable<HttpRequest> _requests;

        /// <summary>
        /// Swagger Request filter
        /// </summary>
        /// <param name="requests"></param>
        public SwaggerRequestFilter(IEnumerable<HttpRequest> requests)
        {
            _requests = requests ?? throw new ArgumentNullException(nameof(requests));
        }
        /// <summary>
        /// Filter out the requests by the specified filters
        /// </summary>
        /// <param name="requestFilter"></param>
        /// <returns></returns>
        public IEnumerable<HttpRequest> FilterRequests(RequestFilter requestFilter)
        {
            IQueryable<HttpRequest> requestsLocal = _requests.AsQueryable();

            if (requestFilter.AuthenticationTypes != null)
            {
                requestsLocal = requestsLocal.Where(request => requestFilter.AuthenticationTypes.Any(x => request.AuthenticationTypes.Contains(x)));
            }

            if (requestFilter.TestTypes != null)
            {
                requestsLocal = requestsLocal.Where(request => requestFilter.TestTypes.Any(x => request.TestTypes.Contains(x)));
            }

            if (requestFilter.EndpointNameWhitelist != null)
            {
                requestsLocal = requestsLocal.Where(request => requestFilter.EndpointNameWhitelist.Any(x => x == request.OperationId));
            }

            if (requestFilter.HttpMethodsWhitelist != null)
            {
                requestsLocal = requestsLocal.Where(request => requestFilter.HttpMethodsWhitelist.Any(x => x == request.Method));
            }

            if (requestFilter.GeneralContains != null)
            {
                requestsLocal = requestsLocal.Where(request => requestFilter.GeneralContains.Any(x =>
                                                    (request.Summary != null && request.Summary.IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                                    (request.Description != null && request.Description.IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                                    (request.Tags != null && request.Tags.Contains(x, StringComparer.InvariantCultureIgnoreCase))));
            }

            return requestsLocal.ToList();
        }
    }
}
