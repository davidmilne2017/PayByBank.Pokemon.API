using Microsoft.Extensions.Logging;
using PayByBank.Pokemon.Common.Constants;
using PayByBank.Pokemon.Common.Domain.Pokemon;
using PayByBank.Pokemon.Common.Domain.Translation;
using PayByBank.Pokemon.Common.ErrorEnums;
using PayByBank.Pokemon.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using PayByBank.Pokemon.Infrastructure.Monitoring.Errors;

namespace PayByBank.Pokemon.Services.Services
{
    public class PokemonService : IPokemonService
    {
        private readonly IPokemonHttpRepository pokemonHttpRepository;
        private readonly ITranslationHttpRepository translationHttpRepository;
        private readonly ILogger<PokemonService> logger;

        public PokemonService(IPokemonHttpRepository pokemonHttpRepository, ITranslationHttpRepository translationHttpRepository, ILogger<PokemonService> logger)
        {
            this.pokemonHttpRepository = pokemonHttpRepository;
            this.translationHttpRepository = translationHttpRepository;
            this.logger = logger;
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
                logger.CustomLogError(ErrorCategory.APPLICATION, ex, ConstantValues.Error_InternalError_Service);
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
                logger.CustomLogError(ErrorCategory.APPLICATION, ex, ConstantValues.Error_InternalError_Service);
                return pokemonResponse.Description;
            }
        }
    }
}
