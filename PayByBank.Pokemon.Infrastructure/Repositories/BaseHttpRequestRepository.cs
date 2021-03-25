using System.Net.Http;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using PayByBank.Pokemon.Infrastructure.Monitoring.Errors;
using PayByBank.Pokemon.Common.ErrorEnums;
using PayByBank.Pokemon.Common.Constants;

namespace PayByBank.Pokemon.Infrastructure.Repositories
{
    public abstract class BaseHttpRequestRepository
    {

        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<BaseHttpRequestRepository> logger;

        protected BaseHttpRequestRepository(IHttpClientFactory httpClientFactory, ILogger<BaseHttpRequestRepository> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        protected abstract ApiSource ApiSource { get; }

        protected async Task<TReturn> GetAsync<TReturn>(string endPoint, CancellationToken cancellationToken)
        { 
            using var request = new HttpRequestMessage(HttpMethod.Get, endPoint);
            var client = httpClientFactory.CreateClient(ApiSource.ToString());
            try
            {
                var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    logger.CustomLogError(ErrorCategory.APPLICATION, string.Format(ConstantValues.Error_ExternalApi_Result, client.BaseAddress, result));
                    return default;
                }

                var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var resp = await JsonSerializer.DeserializeAsync<TReturn>(contentStream, Options, cancellationToken).ConfigureAwait(false);

                return resp;
            }
            catch (Exception ex)
            {
                logger.CustomLogError(ErrorCategory.APPLICATION, string.Format(ConstantValues.Error_ExternalApi_Exception, client.BaseAddress, ex.Message));
                return default;
            }
        }
    }

    public enum ApiSource
    {
        POKEMON,
        TRANSLATION
    }
}
