using FileConvert.Log;
using FileConvert.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace FileConvert.MiddleWare
{
    public class AdminSafeListMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly List<OriginWhiteModel> _adminWhiteList;
        private readonly ILoggerHelper _logger;

        public AdminSafeListMiddleWare(
            RequestDelegate next,
            IOptions<List<OriginWhiteModel>> adminSafeList,
            ILoggerHelper loggerHelper
            )
        {
            _adminWhiteList = adminSafeList.Value;
            _next = next;
            _logger = loggerHelper;
        }

        public async Task Invoke(HttpContext context)
        {

            var origin = context.Request.Headers["Origin"].FirstOrDefault() ?? "";
            var badIp = _adminWhiteList.Where(x => x.Origin == origin).FirstOrDefault() == null ? true : false;
            if (badIp)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                _logger.Debug(typeof(AdminSafeListMiddleWare),"非法域名："+ origin);
                return;
            }
            await _next.Invoke(context);
        }
    }
}
