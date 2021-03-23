using PayByBank.Pokemon.Common.Domain;

namespace PayByBank.Pokemon.Common.Interfaces
{
    public interface IPokemonConverterAdapter
    {
        PokemonResponse ConvertPokemon(string pokemonJson);
    }
}
