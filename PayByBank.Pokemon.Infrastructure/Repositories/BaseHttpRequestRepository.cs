using System.Net.Http;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;

namespace PayByBank.Pokemon.Infrastructure.Repositories
{
    public abstract class BaseHttpRequestRepository
    {

        private readonly IHttpClientFactory httpClientFactory;

        protected BaseHttpRequestRepository(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
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

                    //logger.LogWarning($"Failed to fetch {client.BaseAddress}. {result}");
                    return default;
                }

                var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var resp = await JsonSerializer.DeserializeAsync<TReturn>(contentStream, Options, cancellationToken).ConfigureAwait(false);

                return resp;
            }
            catch (InvalidOperationException ex)
            {
                //logger.LogWarning($"Failed to fetch {client.BaseAddress} with exception: {ex.Message}.");
                return default;
            }
            catch (Exception ex)
            {
                //logger.LogWarning($"Failed to fetch {client.BaseAddress} with exception: {ex.Message}.");
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
