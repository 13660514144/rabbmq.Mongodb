using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace rabbmq.Class
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomExceptionMiddleware> _logger;
        private readonly IHostEnvironment _host;

        public CustomExceptionMiddleware(
            RequestDelegate next,
            ILogger<CustomExceptionMiddleware> logger,
            IHostEnvironment host)
        {
            _next = next;
            _logger = logger;
            _host = host;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private Task HandleException(HttpContext context, Exception exception)
        {
            //_logger.LogError(exception, exception.Message);          
            var body = System.Text.Json.JsonSerializer.Serialize(new
            {
                code = 500,
                message = exception.Message,
                data = new JArray()
            });
           
            var request = context.Request;
            request.EnableBuffering();
            var postJson = "";          
            if (context.Request.Method.ToUpper() == "POST")
            {
                var stream = context.Request.Body;
                long? length = context.Request.ContentLength;
                if (length != null && length > 0)
                {
                    // 使用这个方式读取，并且使用异步
                    StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
                    postJson = streamReader.ReadToEndAsync().Result;
                }
            }
            else
            {
                postJson = HttpUtility.UrlDecode(context.Request.QueryString.ToString());
            }
            LogMongdHelper.ErrorLog(exception, "Gobal", "Gobal", "Gobal",postJson);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            
            return context.Response.WriteAsync(body);
        }
    }
    public static class CustomExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionMiddleware>();
        }
    }
}
