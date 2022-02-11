using Exceptionless;
using FileConvert.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileConvert.Filter
{
    public class GlobalExceptionFilter : IExceptionFilter   
    {
        private readonly IHostEnvironment _env;
        private readonly ILoggerHelper _loggerHelper;

        public GlobalExceptionFilter(IHostEnvironment env, ILoggerHelper loggerHelper)
        {
            _env = env;
            _loggerHelper = loggerHelper;
        }

        public void OnException(ExceptionContext context)
        {
            var json = new JsonErrorResponse();
            json.Message = context.Exception.Message;
            if (_env.IsDevelopment())
            {
                json.DevelopmentMessage = context.Exception.StackTrace;
            }
            context.Result = new InternalServerErrorObjectResult(json);

            _loggerHelper.Error(context.Exception.Source, json.Message, context.Exception);
            context.Exception.ToExceptionless().Submit();
        }

        public class InternalServerErrorObjectResult : ObjectResult
        {
            public InternalServerErrorObjectResult(object value) : base(value)
            {
                StatusCode = StatusCodes.Status500InternalServerError;
            }

        }

        //返回错误信息
        public class JsonErrorResponse
        {
            /// <summary>
            /// 生产环境的消息
            /// </summary>
            public string Message { get; set; }
            /// <summary>
            /// 开发环境的消息
            /// </summary>
            public string DevelopmentMessage { get; set; }
        }
    }
}
