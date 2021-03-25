using Microsoft.Extensions.Configuration;
using PayByBank.Pokemon.Common.Constants;
using PayByBank.Pokemon.Common.Domain.Translation;
using PayByBank.Pokemon.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace PayByBank.Pokemon.Infrastructure.Repositories
{
    public class TranslationHttpRepository : BaseHttpRequestRepository, ITranslationHttpRepository
    {
        protected override ApiSource ApiSource => ApiSource.TRANSLATION;
        private readonly string Resource;

        public TranslationHttpRepository(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
            : base(httpClientFactory)
        {
            this.Resource = configuration.GetValue<string>(Constants.TranslationApi);
        }

        public async Task<string> TranslateText(string text, TranslationType translationType, CancellationToken cancellationToken)
        {
            try
            {
                var builder = new UriBuilder($"{Resource}{translationType.ToString().ToLower()}");
                var query = HttpUtility.ParseQueryString(builder.Query);
                query["text"] = text;
                builder.Query = query.ToString();
                var endPoint = builder.ToString();
                
                var translationResponse = await GetAsync<TranslationResponse>(endPoint, cancellationToken).ConfigureAwait(false);

                if (translationResponse == null || (translationResponse.Success?.Total ?? 0) == 0)
                    return text;

                return (translationResponse.Contents?.Translated ?? text);
                
            }
            catch
            {
                return default;
            }
        }
    }    
}
