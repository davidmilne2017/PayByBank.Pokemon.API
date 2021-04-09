using PayByBank.Pokemon.Common.Domain.Translation;
using PayByBank.Pokemon.Common.Interfaces;
using System;

namespace PayByBank.Pokemon.Domain.Pokemon.PokemonTypes
{
    public class LegendaryPokemon : IPokemonResponse
    {
        public string Habitat { get => decoratedPokemonResponse.Habitat; set => decoratedPokemonResponse.Habitat = value; }
        public bool IsLegendary { get => decoratedPokemonResponse.IsLegendary; set => decoratedPokemonResponse.IsLegendary = value; }
        public TranslationType TranslationType { get => decoratedPokemonResponse.TranslationType; set => decoratedPokemonResponse.TranslationType = value; }

        private IPokemonResponse decoratedPokemonResponse { get; set; }

        public LegendaryPokemon(IPokemonResponse pokemonResponse)
        {
            this.decoratedPokemonResponse = pokemonResponse;
            if (IsLegendary)
                TranslationType = TranslationType.YODA;
        }

    }
}
