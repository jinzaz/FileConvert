using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace FileConvertTests.Extensions
{
    static class HttpClientExtensions
    {
        private const string Origin = "isrcauat.iqdii.com";
        public static HttpClient CreateIdempotentClient(this TestServer server)
        {
            var client = server.CreateClient();
            client.DefaultRequestHeaders.Add("x-requestid", Guid.NewGuid().ToString());
            client.DefaultRequestHeaders.Add("Origin", Origin);
            return client;
        }
    }
}
