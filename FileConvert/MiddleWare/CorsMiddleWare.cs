using Exceptionless;
using FileConvert.Log;
using FileConvert.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace FileConvert.MiddleWare
{
    public class CorsMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly List<OriginWhiteModel> _adminWhiteList;
        private readonly ILoggerHelper _logger;
        private readonly IWebHostEnvironment _env;
        public CorsMiddleWare(RequestDelegate next,
                              IOptions<List<OriginWhiteModel>> adminSafeList,
                              ILoggerHelper loggerHelper,
                              IWebHostEnvironment env)
        {
            _next = next;
            _adminWhiteList = adminSafeList.Value;
            _logger = loggerHelper;
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                if (!context.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
                {
                    context.Response.Headers.Add("Access-Control-Allow-Origin", context.Request.Headers["Origin"]);
                    context.Response.Headers.Add("Access-Control-Allow-Headers", context.Request.Headers["Access-Control-Request-Headers"]);
                    context.Response.Headers.Add("Access-Control-Allow-Methods", "PUT, POST, GET, DELETE");
                    context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                    if (context.Request.Method.Equals("OPTIONS"))
                    {
                        context.Response.StatusCode = StatusCodes.Status200OK;
                        return;
                    }
                }
                if (_env.IsProduction())
                {
                    var origin = context.Request.Headers["Origin"].FirstOrDefault() ?? "";
                    var badIp = _adminWhiteList.Where(x => origin.Contains(x.Origin)).FirstOrDefault() == null ? true : false;
                    if (badIp)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        _logger.Debug(typeof(CorsMiddleWare), "非法域名：" + origin);
                        ExceptionlessClient.Default.SubmitLog(typeof(CorsMiddleWare).FullName, $"非法域名：{origin}");
                        return;
                    }
                }
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.Error(typeof(CorsMiddleWare),ex);
            }

        }
    }
}
