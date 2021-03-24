using PayByBank.Pokemon.Common.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace PayByBank.Pokemon.Common.Interfaces
{
    public interface IPokemonHttpRepository
    {
        public Task<PokemonResponse> FindPokemonAsync(string pokemonName, CancellationToken cancellationToken);
    }
}
