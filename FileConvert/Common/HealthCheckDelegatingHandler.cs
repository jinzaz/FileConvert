using log4net.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FileConvert.Common
{
    public class HealthCheckDelegatingHandler :DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<HealthCheckDelegatingHandler> _logger;
        private const string ProductionOrigin = "https://compressionservice.iqdii.com";
        public HealthCheckDelegatingHandler(IHttpContextAccessor httpContextAccessor,ILogger<HealthCheckDelegatingHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequest,CancellationToken cancellationToken)
        {
            HttpResponseMessage httpResponseMessage;
            try
            {
                httpRequest.Headers.Add("Origin", ProductionOrigin);
                httpResponseMessage = await base.SendAsync(httpRequest, cancellationToken);
                httpResponseMessage.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw;
            }
            return httpResponseMessage;
        }

    }
}
