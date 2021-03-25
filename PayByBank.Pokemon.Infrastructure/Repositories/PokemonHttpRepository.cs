using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PayByBank.Pokemon.Common.Constants;
using PayByBank.Pokemon.Common.Domain.Pokemon;
using PayByBank.Pokemon.Common.ErrorEnums;
using PayByBank.Pokemon.Common.Interfaces;
using PayByBank.Pokemon.Infrastructure.Monitoring.Errors;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PayByBank.Pokemon.Infrastructure.Repositories
{
    public class PokemonHttpRepository : BaseHttpRequestRepository, IPokemonHttpRepository
    {
        private readonly IPokemonConverterAdapter pokemonConverterAdapter;
        private readonly ILogger<PokemonHttpRepository> logger;

        protected override ApiSource ApiSource => ApiSource.POKEMON;

        private readonly string Resource;

        public PokemonHttpRepository(
            IHttpClientFactory httpClientFactory,
            IPokemonConverterAdapter pokemonConverterAdapter,
            IConfiguration configuration,
            ILogger<PokemonHttpRepository> logger)
            : base(httpClientFactory, logger)
        {
            this.pokemonConverterAdapter = pokemonConverterAdapter;
            this.Resource = configuration.GetValue<string>(ConstantValues.PokemonApi);
            this.logger = logger;
        }

        public async Task<PokemonResponse> FindPokemonAsync(string pokemonName, CancellationToken cancellationToken)
        {
            try
            {
                var endPoint = $"{Resource}/{pokemonName}";
                var pokemonApiResponse =  await GetAsync<PokemonApiReturn>(endPoint, cancellationToken).ConfigureAwait(false);

                if (pokemonApiResponse == null)
                    return null;
                
                var pokemonResponse = pokemonConverterAdapter.ConvertPokemon(pokemonApiResponse);
                return pokemonResponse;
            }
            catch (Exception ex)
            {
                logger.CustomLogError(ErrorCategory.APPLICATION, ex, ConstantValues.Error_InternalError_Repository);
                return default;
            }            
        }
    }
}
