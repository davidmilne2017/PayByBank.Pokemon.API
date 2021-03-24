using System.Linq;
using PayByBank.Pokemon.Common.Domain;
using PayByBank.Pokemon.Common.Interfaces;
using System;

namespace PayByBank.Pokemon.Infrastructure.Adapters
{
    public class PokemonConverterAdapter : IPokemonConverterAdapter
    {

        private const string language = "en";

        public PokemonResponse ConvertPokemon(PokemonApiReturn pokemon)
        {

            try
            {
                var pokemonResponse = new PokemonResponse()
                {
                    Name = pokemon.names.FirstOrDefault(x => x.language.name == language).name,
                    IsLegendary = pokemon.is_legendary,
                    Habitat = pokemon.habitat.name,
                    Description = pokemon.flavor_text_entries.FirstOrDefault(x => x.language.name == language).flavor_text
                };

                return pokemonResponse;
            }
            catch (Exception ex)
            {
                //add logging code
                return default;
            };
        }
    }
}
