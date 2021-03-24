using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using AutoFixture;
using PayByBank.Pokemon.Common.Interfaces;
using PayByBank.Pokemon.Services.Services;
using System.Threading;
using PayByBank.Pokemon.Common.Domain;
using System;

namespace PayByBank.Pokemon.Services.Tests.Services
{
    public class PokemonServiceTests
    {

        private readonly Fixture fixture;

        public PokemonServiceTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public async Task SearchPokemonAsync_PokemonReturned()
        {
            //Arrange
            var pokemonHttpRepositoryMock = new Mock<IPokemonHttpRepository>();
            var sut = new PokemonService(pokemonHttpRepositoryMock.Object);
            var token = new CancellationToken();
            var expPokemon = fixture.Create<PokemonResponse>();
            pokemonHttpRepositoryMock.Setup(x => x.FindPokemonAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(expPokemon);

            //Act
            var result = await sut.SearchPokemonAsync("", token);

            //Assert
            result.Should().NotBeNull();
            result.Should().Be(expPokemon);
        }

        [Fact]
        public async Task SearchPokemonAsync_ThrowsException_ReturnsNull()
        {
            //Arrange
            var pokemonHttpRepositoryMock = new Mock<IPokemonHttpRepository>();
            var sut = new PokemonService(pokemonHttpRepositoryMock.Object);
            var token = new CancellationToken();
            pokemonHttpRepositoryMock.Setup(x => x.FindPokemonAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Throws(new Exception());

            //Act
            var result = await sut.SearchPokemonAsync("", token);

            //Assert
            result.Should().BeNull();
        }

    }
}
