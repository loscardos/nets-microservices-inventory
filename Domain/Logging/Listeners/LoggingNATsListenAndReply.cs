using InventoryService.Constants.Logger;
using InventoryService.Domain.Logging.Services;
using InventoryService.Infrastructure.Subscriptions;

namespace InventoryService.Domain.Logging.Listeners
{
    public class LoggingNATsListenAndReply(
        ILoggerFactory loggerFactory,
        LoggingService loggingService
    ) : IReplyAction<IDictionary<string, object>, IDictionary<string, object>>
    {

        public readonly ILogger _logger = loggerFactory.CreateLogger(LoggerConstant.NATS);
        public readonly LoggingService _loggingService = loggingService;

        public IDictionary<string, object> Reply(IDictionary<string, object> data)
        {
            // EXAMPLE: Do operation for reply event

            var reply = new Dictionary<string, object> {
                { "status", "OK" },
            };

            return reply;
        }
    }
}
