using System.Threading;
using System.Threading.Tasks;
using PayByBank.Pokemon.Common.Domain;

namespace PayByBank.Pokemon.Common.Interfaces
{
    public interface IPokemonHttpRepository
    {
        public Task<PokemonResponse> FindPokemon(string pokemonName, CancellationToken cancellationToken);
    }
}
