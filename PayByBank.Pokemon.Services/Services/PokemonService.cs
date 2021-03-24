using PayByBank.Pokemon.Common.Domain;
using PayByBank.Pokemon.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PayByBank.Pokemon.Services.Services
{
    public class PokemonService : IPokemonService
    {

        private readonly IPokemonHttpRepository pokemonHttpRepository;

        public PokemonService(IPokemonHttpRepository pokemonHttpRepository)
        {
            this.pokemonHttpRepository = pokemonHttpRepository;
        }

        public async Task<PokemonResponse> SearchPokemonAsync(string pokemonName, CancellationToken cancellationToken)
        {
            try
            {
                return await pokemonHttpRepository.FindPokemonAsync(pokemonName, cancellationToken);
            }
            catch(Exception ex)
            {
                //add logging
                return default;
            }
            
        }
    }
}
