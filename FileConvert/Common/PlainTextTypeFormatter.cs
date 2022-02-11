using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileConvert.Common
{
    public class PlainTextTypeFormatter : TextInputFormatter
    {
        public PlainTextTypeFormatter()
        {
            SupportedMediaTypes.Add("text/plain");
            SupportedEncodings.Add(System.Text.Encoding.UTF8);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            string content;
            using (var reader = context.ReaderFactory(context.HttpContext.Request.Body, encoding))
            {
                content = await reader.ReadToEndAsync();
            }
            return await InputFormatterResult.SuccessAsync(content);
        }
    }
}
