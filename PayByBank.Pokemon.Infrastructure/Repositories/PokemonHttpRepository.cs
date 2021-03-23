using PayByBank.Pokemon.Common.Interfaces;
using System.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PayByBank.Pokemon.Infrastructure.Repositories
{
    public class PokemonHttpRepository : BaseHttpRequestRepository, IPokemonHttpRepository
    {
        protected override ApiSource ApiSource => ApiSource.POKEMON;

        protected override string Resource => ConfigurationManager.AppSettings["PokemonApi"];

        public PokemonHttpRepository(
            IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
        }

        public async Task<string> FindPokemonAsync(string pokemonName, CancellationToken cancellationToken)
        {
            var endPoint = $"{Resource}/{pokemonName}";
            return await GetAsync<string>(endPoint, cancellationToken).ConfigureAwait(false);
        }
    }
}
