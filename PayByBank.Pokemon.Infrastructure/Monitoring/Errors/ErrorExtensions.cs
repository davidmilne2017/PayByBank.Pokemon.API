using Microsoft.Extensions.Logging;
using PayByBank.Pokemon.Common.ErrorEnums;
using System;

namespace PayByBank.Pokemon.Infrastructure.Monitoring.Errors
{
    public static class ErrorExtensions
    {
        public static string Label(this ErrorCategory value)
            => value.ToString().ToLower();

        public static string Label(this ErrorLevel value)
            => value.ToString().ToLower();


        public static void CustomLogError(this ILogger logger, ErrorCategory category, string message)
        {
            PokemonMetrics.ErrorsCounter.WithLabels(ErrorLabels.Labels(category, ErrorLevel.ERROR)).Inc();
            logger.LogError(message);
        }

        public static void CustomLogError(this ILogger logger, ErrorCategory category, Exception exception, string message)
        {
            PokemonMetrics.ErrorsCounter.WithLabels(ErrorLabels.Labels(category, ErrorLevel.ERROR)).Inc();
            logger.LogError(exception, message);
        }

        public static void CustomLogWarning(this ILogger logger, ErrorCategory category, string message)
        {
            PokemonMetrics.ErrorsCounter.WithLabels(ErrorLabels.Labels(category, ErrorLevel.WARNING)).Inc();
            logger.LogError(message);
        }

    }
}
