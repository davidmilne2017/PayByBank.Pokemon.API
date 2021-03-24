using PayByBank.Pokemon.Common.Domain;
using PayByBank.Pokemon.Common.Interfaces;
using System;
using System.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PayByBank.Pokemon.Infrastructure.Repositories
{
    public class PokemonHttpRepository : BaseHttpRequestRepository, IPokemonHttpRepository
    {
        private readonly IPokemonConverterAdapter pokemonConverterAdapter;

        protected override ApiSource ApiSource => ApiSource.POKEMON;

        protected override string Resource => ConfigurationManager.AppSettings["PokemonApi"];

        public PokemonHttpRepository(
            IHttpClientFactory httpClientFactory,
            IPokemonConverterAdapter pokemonConverterAdapter)
            : base(httpClientFactory)
        {
            this.pokemonConverterAdapter = pokemonConverterAdapter;
        }

        public async Task<PokemonResponse> FindPokemonAsync(string pokemonName, CancellationToken cancellationToken)
        {
            try
            {
                var endPoint = $"{Resource}/{pokemonName}";
                var pokemonApiResponse =  await GetAsync<PokemonApiReturn>(endPoint, cancellationToken).ConfigureAwait(false);
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
