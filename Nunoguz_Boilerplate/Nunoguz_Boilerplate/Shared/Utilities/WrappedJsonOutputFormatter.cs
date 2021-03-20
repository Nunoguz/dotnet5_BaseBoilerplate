using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using System.Buffers;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Nunoguz_Boilerplate.Shared.Utilities
{
    public class WrappedJsonOutputFormatter : NewtonsoftJsonOutputFormatter
    {
        private const string RESPONSE_HEADER_RESPONSE_TIME = "X-Response-Time-ms";
        public string Version { get; set; }

        public WrappedJsonOutputFormatter(JsonSerializerSettings serializerSettings, ArrayPool<char> charPool, MvcOptions options)
            : base(serializerSettings, charPool, options)
        { }


        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            if (context.HttpContext.Response.StatusCode == (int)HttpStatusCode.OK)
            {
                var @object = new ApiResponse { Data = context.Object, ApiVersion = Version };
                var newContext = new OutputFormatterWriteContext(context.HttpContext, context.WriterFactory, typeof(ApiResponse), @object)
                {
                    ContentType = context.ContentType,
                    ContentTypeIsServerDefined = context.ContentTypeIsServerDefined
                };

                //var responseTime = DateTime.Now - (DateTime)context.HttpContext.Items["RequestTime"];
                //@object.ResponseTime = responseTime.TotalMilliseconds;

                return base.WriteResponseBodyAsync(newContext, selectedEncoding);
            }
            else
            {

            }

            return base.WriteResponseBodyAsync(context, selectedEncoding);
        }
    }
}
