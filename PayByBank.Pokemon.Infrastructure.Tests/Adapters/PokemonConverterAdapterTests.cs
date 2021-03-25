using PayByBank.Pokemon.Infrastructure.Adapters;
using FluentAssertions;
using System.Text.Json;
using System.Linq;
using Xunit;
using PayByBank.Pokemon.Common.Domain.Pokemon;

namespace PayByBank.Pokemon.Services.Tests.Adapters
{
    public class PokemonConverterAdapterTests
    {

        private const string language = "en";

        [Fact]
        public void ConvertPokemon_WithValidPokemon_ReturnsCorrectObject()
        {
            //Arrange
            var pokemonReturn = TestPokemon();
            var sut = new PokemonConverterAdapter();
            var expDescription = pokemonReturn.flavor_text_entries.FirstOrDefault(x => x.language.name == language).flavor_text
                .Replace("\n", " ")
                .Replace("\f", " ")
                .Replace("\r", " ");

            //Act
            var response = sut.ConvertPokemon(pokemonReturn);

            //Assert
            response.Should().BeOfType<PokemonResponse>();
            response.Name.Should().Be(pokemonReturn.names.FirstOrDefault(x => x.language.name == language).name);
            response.IsLegendary.Should().Be(pokemonReturn.is_legendary);
            response.Habitat.Should().Be(pokemonReturn.habitat.name);
            response.Description.Should().Be(expDescription);
        }

        [Fact]
        public void ConvertPokemon_WithInvalidPokemon_ReturnsNull()
        {
            //Arrange
            var pokemonReturn = new PokemonApiReturn();
            var sut = new PokemonConverterAdapter();

            //Act
            var response = sut.ConvertPokemon(pokemonReturn);

            //Assert
            response.Should().BeNull();
        }

        private PokemonApiReturn TestPokemon()
        {
            var pokemonJson = Common.Resources.Pokemon.ResourceManager.GetString("pikachu");
            return JsonSerializer.Deserialize<PokemonApiReturn>(pokemonJson);
        }

    }
}
