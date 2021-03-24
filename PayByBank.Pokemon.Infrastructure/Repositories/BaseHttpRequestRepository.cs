using System.Net.Http;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Net.Http.Headers;
using PayByBank.Pokemon.Common.Domain;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;

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

        protected abstract string Resource { get; }

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

        protected async Task<TReturn> PostAsync<TCreate, TReturn>(string endPoint, TCreate entity, CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, endPoint);
            var client = httpClientFactory.CreateClient(ApiSource.ToString());
            try
            {
                var json = JsonSerializer.Serialize(entity);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

                if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    return await JsonSerializer.DeserializeAsync<TReturn>(contentStream, Options, cancellationToken).ConfigureAwait(false);
                }

                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var errorResponse = JsonSerializer.Deserialize<ErrorApiResponse>(result, Options);
                var errorMessage = GetErrorMessages(errorResponse);

                //logger.LogWarning($"Failed to fetch {client.BaseAddress} ( Status Code: {response.StatusCode} ). {result}");
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.BadRequest:
                        throw new ValidationException(errorMessage);
                    case System.Net.HttpStatusCode.Forbidden:
                        throw new AuthenticationException(errorMessage);
                    default:
                        throw new ArgumentException(errorMessage);
                }
            }
            catch (InvalidOperationException ex)
            {
                //logger.LogWarning($"Failed to fetch {client.BaseAddress} with exception: {ex.Message}.");
                return default;
            }
        }

        private static string GetErrorMessages(ErrorApiResponse errorResponse)
        {
            var errorMessage = new StringBuilder();
            foreach (var item in errorResponse.Errors)
            {
                errorMessage.Append(string.Join(" . ", item.Value));
            }

            return errorMessage.ToString();
        }
    }

    public enum ApiSource
    {
        POKEMON,
        TRANSLATIONYODA,
        TRANSLATIONSHAKESPEARE
    }
}
