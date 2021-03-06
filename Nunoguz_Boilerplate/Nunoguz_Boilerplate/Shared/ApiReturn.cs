using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nunoguz_Boilerplate.Shared
{
    public class ApiReturn
    {
        public string ApiVersion { get; set; } = "1.0";
        public int StatusCode { get; set; }
        public double ResponseTime { get; set; }
        public DateTime UtcTimestamp { get; set; } = DateTime.UtcNow;
        public object Data { get; set; }
        public bool IsErrorOccured { get; set; } = false;
        public Error Error { get; set; }

        public bool ShouldSerializeError()
        {
            return IsErrorOccured;
        }

        public bool ShouldSerializeData()
        {
            return !IsErrorOccured;
        }

        public static ApiReturn ErrorResponse(Error error, int statusCode)
        {
            return new ApiReturn
            {
                Error = error,
                IsErrorOccured = true,
                StatusCode = statusCode
            };
        }
    }
}
