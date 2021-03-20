using Microsoft.Extensions.Logging;

namespace Nunoguz_Boilerplate.Shared.Utilities
{
    public class LoggerBase<T> where T : class
    {
        private readonly ILogger<T> _logger;
        public LoggerBase(ILogger<T> logger)
        {
            _logger = logger;
        }
    }
}
