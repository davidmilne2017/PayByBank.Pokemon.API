using PayByBank.Pokemon.Common.Domain.Pokemon;
using PayByBank.Pokemon.Common.Domain.Translation;
using PayByBank.Pokemon.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PayByBank.Pokemon.Services.Services
{
    public class PokemonService : IPokemonService
    {
        private readonly IPokemonHttpRepository pokemonHttpRepository;
        private readonly ITranslationHttpRepository translationHttpRepository;

        public PokemonService(IPokemonHttpRepository pokemonHttpRepository, ITranslationHttpRepository translationHttpRepository)
        {
            this.pokemonHttpRepository = pokemonHttpRepository;
            this.translationHttpRepository = translationHttpRepository;
        }

        public async Task<PokemonResponse> SearchPokemonAsync(string pokemonName, bool translate, CancellationToken cancellationToken)
        {
            try
            {
                var pokemonResponse = await pokemonHttpRepository.FindPokemonAsync(pokemonName, cancellationToken);
                if (translate)
                    pokemonResponse.Description = await TranslatePokemonAsync(pokemonResponse, cancellationToken);                

                return pokemonResponse;
            }
            catch(Exception ex)
            {
                //add logging
                return default;
            }            
        }

        private async Task<string> TranslatePokemonAsync(PokemonResponse pokemonResponse, CancellationToken cancellationToken)
        {
            try
            {
                var translationType = pokemonResponse.Habitat.ToLower() == "cave" || pokemonResponse.IsLegendary ? TranslationType.YODA : TranslationType.SHAKESPEARE;
                return await translationHttpRepository.TranslateText(pokemonResponse.Description, translationType, cancellationToken);
            }
            catch (Exception ex)
            {
                //add logging
                return pokemonResponse.Description;
            }
        }
    }
}
