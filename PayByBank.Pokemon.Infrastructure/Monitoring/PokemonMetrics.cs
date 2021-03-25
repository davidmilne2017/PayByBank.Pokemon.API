using PayByBank.Pokemon.Infrastructure.Monitoring.Errors;
using Prometheus;

namespace PayByBank.Pokemon.Infrastructure.Monitoring
{
    public class PokemonMetrics
    {

        public static Counter ErrorsCounter { get; } = Metrics.CreateCounter(
            "PokemonErrors",
            "Errors raised",
            new CounterConfiguration
            {
                LabelNames = ErrorLabels.LabelNames
            });
    }
}
