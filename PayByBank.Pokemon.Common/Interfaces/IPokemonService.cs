using PayByBank.Pokemon.Common.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace PayByBank.Pokemon.Common.Interfaces
{
    public interface IPokemonService
    {
        Task<PokemonResponse> SearchPokemonAsync(string pokemonName, CancellationToken cancellationToken);
    }
}
