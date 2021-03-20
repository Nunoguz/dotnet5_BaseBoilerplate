using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Nunoguz_Boilerplate.Shared.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private static string _version;

        public ErrorHandlerMiddleware(RequestDelegate next, string version)
        {
            this.next = next;
            _version = version;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (ApiException exception)
            {
                var code = exception switch
                {
                    ApiException => HttpStatusCode.InternalServerError,
                    _ => HttpStatusCode.BadRequest
                };

                await HandleExceptionAsync(context, exception, code);
            }
            catch (Exception ex)
            {
                var error = new Error
                {
                    Message = $"Undefined error occured. Message: {ex.Message}",
                    StackTrace = ex.StackTrace
                };

                var code = ex switch
                {
                    ApiException => HttpStatusCode.InternalServerError,
                    KeyNotFoundException => HttpStatusCode.NotFound,
                    _ => HttpStatusCode.BadRequest
                };

                await HandleExceptionAsync(context, new ApiException(error), code);
            }
        }
        private static Task HandleExceptionAsync(HttpContext context, ApiException exception, HttpStatusCode code)
        {
            var resp = ApiReturn.ErrorResponse(exception.Error, (int)code);
            resp.ApiVersion = _version;
            var result = JsonConvert.SerializeObject(resp);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
