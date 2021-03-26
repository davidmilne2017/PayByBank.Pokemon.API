using System.Linq;
using PayByBank.Pokemon.Common.Interfaces;
using System;
using PayByBank.Pokemon.Common.Domain.Pokemon;
using Microsoft.Extensions.Logging;
using PayByBank.Pokemon.Infrastructure.Monitoring.Errors;
using PayByBank.Pokemon.Common.ErrorEnums;
using PayByBank.Pokemon.Common.Constants;

namespace PayByBank.Pokemon.Infrastructure.Adapters
{
    public class PokemonConverterAdapter : IPokemonConverterAdapter
    {
        private const string language = "en";
        private readonly ILogger<PokemonConverterAdapter> logger;

        public PokemonConverterAdapter(ILogger<PokemonConverterAdapter> logger)
        {
            this.logger = logger;
        }

        public PokemonResponse ConvertPokemon(PokemonApiReturn pokemon)
        {

            try
            {
                var pokemonResponse = new PokemonResponse()
                {
                    Name = pokemon.names.FirstOrDefault(x => x.language.name == language).name,
                    IsLegendary = pokemon.is_legendary,
                    Habitat = pokemon.habitat?.name ?? "",
                    Description = pokemon.flavor_text_entries.FirstOrDefault(x => x.language.name == language).flavor_text
                        .Replace("\n", " ")
                        .Replace("\f", " ")
                        .Replace("\r", " ")
                };

                return pokemonResponse;
            }
            catch (Exception ex)
            {
                logger.CustomLogError(ErrorCategory.APPLICATION, ex, ConstantValues.Error_InternalError_Adapter);
                return default;
            }
        }
    }
}
