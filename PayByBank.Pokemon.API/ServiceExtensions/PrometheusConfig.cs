using PayByBank.Pokemon.Common.Constants;
using Prometheus;
using Prometheus.HttpMetrics;


namespace PayByBank.Pokemon.API.ServiceExtensions
{
    public static class PrometheusConfig
    {
        public static HttpMiddlewareExporterOptions Options => new HttpMiddlewareExporterOptions
        {
            InProgress = new HttpInProgressOptions
            {
                Enabled = true,
                Gauge = Metrics.CreateGauge(
                    "http_requests_in_progress",
                    "The number of requests currently in progress in the ASP.NET Core pipeline. One series without controller/action label values counts all in-progress requests, with separate series existing for each controller-action pair.",
                    new string[3]
                    {
                        HttpRequestLabelNames.Method,
                        HttpRequestLabelNames.Controller,
                        HttpRequestLabelNames.Action
                    }
                )
            },
            RequestCount = new HttpRequestCountOptions
            {
                Enabled = true,
                Counter = Metrics.CreateCounter(
                    "http_requests_received_total",
                    "Provides the count of HTTP requests that have been processed by the ASP.NET Core pipeline.",
                    new string[4]
                    {
                        HttpRequestLabelNames.Code,
                        HttpRequestLabelNames.Method,
                        HttpRequestLabelNames.Controller,
                        HttpRequestLabelNames.Action
                    })
            },

            RequestDuration = new HttpRequestDurationOptions
            {
                Enabled = true,
                Histogram = Metrics.CreateHistogram(
                    "http_request_duration_seconds",
                    "The duration of HTTP requests processed by an ASP.NET Core application.",
                    new HistogramConfiguration()
                    {
                        Buckets = ConstantValues.smallBucketsSeconds,
                        LabelNames = new string[4]
                        {
                            HttpRequestLabelNames.Code,
                            HttpRequestLabelNames.Method,
                            HttpRequestLabelNames.Controller,
                            HttpRequestLabelNames.Action
                        }
                    })
            }
        };
    }
}
