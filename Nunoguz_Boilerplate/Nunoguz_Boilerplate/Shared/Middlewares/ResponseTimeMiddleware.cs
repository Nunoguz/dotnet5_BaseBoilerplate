using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Nunoguz_Boilerplate.Shared.Middlewares
{
    public class ResponseTimeMiddleware
    {
        private const string RESPONSE_HEADER_RESPONSE_TIME = "X-Response-Time-ms";
        // Handle to the next Middleware in the pipeline  
        private readonly RequestDelegate _next;
        public ResponseTimeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task InvokeAsync(HttpContext context)
        {
            context.Items["RequestTime"] = DateTime.Now;

            // Call the next delegate/middleware in the pipeline   
            return this._next(context);
        }
    }
}
