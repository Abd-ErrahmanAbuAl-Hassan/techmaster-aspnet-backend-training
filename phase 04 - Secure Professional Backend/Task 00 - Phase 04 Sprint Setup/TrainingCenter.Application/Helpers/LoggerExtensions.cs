using Microsoft.Extensions.Logging;

namespace TrainingCenter.Application.Helpers
{
    public static class LoggerExtensions
    {
        public static void LogServiceCall(this ILogger logger, string serviceName, string methodName, object parameters = null)
        {
            logger.LogInformation("Service call: {ServiceName}.{MethodName}. Parameters: {@Parameters}",
                serviceName, methodName, parameters);
        }

        public static void LogServiceError(this ILogger logger, string serviceName, string methodName, Exception ex)
        {
            logger.LogError(ex, "Error in service call: {ServiceName}.{MethodName}. Exception: {@Exception}",
                serviceName, methodName, ex);
        }

        public static void LogValidationError(this ILogger logger, string context, List<string> errors)
        {
            logger.LogWarning("Validation failed in {Context}. Errors: {@Errors}",
                context, errors);
        }

        public static void LogResourceNotFound(this ILogger logger, string resourceType, object identifier)
        {
            logger.LogInformation("Resource not found. Type: {ResourceType}, Identifier: {@Identifier}",
                resourceType, identifier);
        }

        public static void LogBusinessLogicError(this ILogger logger, string message, Exception ex = null)
        {
            if (ex != null)
                logger.LogWarning(ex, "Business logic error: {Message}. Exception: {@Exception}", message, ex);
            else
                logger.LogWarning("Business logic error: {Message}", message);
        }
    }
}