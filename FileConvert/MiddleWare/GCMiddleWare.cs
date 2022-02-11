using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FileConvert.MiddleWare
{
    public class GCMiddleWare
    {
        private readonly RequestDelegate _next;

        public GCMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            await _next(httpContext);
            GC.Collect(2,GCCollectionMode.Forced,true);
            GC.WaitForPendingFinalizers();
        }
    }
}
