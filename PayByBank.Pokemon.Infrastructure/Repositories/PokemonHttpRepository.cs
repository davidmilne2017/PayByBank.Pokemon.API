using Microsoft.Extensions.Configuration;
using PayByBank.Pokemon.Common.Constants;
using PayByBank.Pokemon.Common.Domain.Pokemon;
using PayByBank.Pokemon.Common.Interfaces;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PayByBank.Pokemon.Infrastructure.Repositories
{
    public class PokemonHttpRepository : BaseHttpRequestRepository, IPokemonHttpRepository
    {
        private readonly IPokemonConverterAdapter pokemonConverterAdapter;

        protected override ApiSource ApiSource => ApiSource.POKEMON;

        private readonly string Resource;

        public PokemonHttpRepository(
            IHttpClientFactory httpClientFactory,
            IPokemonConverterAdapter pokemonConverterAdapter,
            IConfiguration configuration)
            : base(httpClientFactory)
        {
            this.pokemonConverterAdapter = pokemonConverterAdapter;
            this.Resource = configuration.GetValue<string>(Constants.PokemonApi);
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
            catch
            {
                return default;
            }            
        }
    }
}
