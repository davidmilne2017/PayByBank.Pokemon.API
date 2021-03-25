using PayByBank.Pokemon.Common.Domain.Pokemon;

namespace PayByBank.Pokemon.Common.Interfaces
{
    public interface IPokemonConverterAdapter
    {
        PokemonResponse ConvertPokemon(PokemonApiReturn pokemon);
    }
}
