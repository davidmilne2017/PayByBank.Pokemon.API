using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PayByBank.Pokemon.Common.Constants;
using PayByBank.Pokemon.Common.Domain.Translation;
using PayByBank.Pokemon.Common.ErrorEnums;
using PayByBank.Pokemon.Common.Interfaces;
using PayByBank.Pokemon.Infrastructure.Monitoring.Errors;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace PayByBank.Pokemon.Infrastructure.Repositories
{
    public class TranslationHttpRepository : BaseHttpRequestRepository, ITranslationHttpRepository
    {
        protected override ApiSource ApiSource => ApiSource.TRANSLATION;
        private readonly string Resource;
        private readonly ILogger<TranslationHttpRepository> logger;

        public TranslationHttpRepository(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<TranslationHttpRepository> logger)
            : base(httpClientFactory, logger)
        {
            this.Resource = configuration.GetValue<string>(ConstantValues.TranslationApi);
            this.logger = logger;
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
            catch (Exception ex)
            {
                logger.CustomLogError(ErrorCategory.APPLICATION, ex, ConstantValues.Error_InternalError_Repository);
                return default;
            }
        }
    }    
}
