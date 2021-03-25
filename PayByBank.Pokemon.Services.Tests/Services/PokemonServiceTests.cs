using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using AutoFixture;
using PayByBank.Pokemon.Common.Interfaces;
using PayByBank.Pokemon.Services.Services;
using System.Threading;
using PayByBank.Pokemon.Common.Domain.Pokemon;
using System;
using PayByBank.Pokemon.Common.Domain.Translation;
using Microsoft.Extensions.Logging;

namespace PayByBank.Pokemon.Services.Tests.Services
{
    public class PokemonServiceTests
    {

        private readonly Fixture fixture;
        private const string yodaTranslation = "Translated, it is";
        private const string shakespeareTranslation = "'tis translated";

        public PokemonServiceTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public async Task SearchPokemonAsync_NoTranslation_PokemonReturned()
        {
            //Arrange
            var pokemonHttpRepositoryMock = new Mock<IPokemonHttpRepository>();
            var translationHttpRepositoryMock = new Mock<ITranslationHttpRepository>();
            var loggerMock = new Mock<ILogger<PokemonService>>();
            var sut = new PokemonService(pokemonHttpRepositoryMock.Object, translationHttpRepositoryMock.Object, loggerMock.Object);
            var token = new CancellationToken();
            var expPokemon = fixture.Create<PokemonResponse>();
            pokemonHttpRepositoryMock.Setup(x => x.FindPokemonAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(expPokemon);

            //Act
            var result = await sut.SearchPokemonAsync("",false, token);

            //Assert
            result.Should().NotBeNull();
            result.Should().Be(expPokemon);
        }

        [Fact]
        public async Task SearchPokemonAsync_NoTranslation_ThrowsException_ReturnsNull()
        {
            //Arrange
            var pokemonHttpRepositoryMock = new Mock<IPokemonHttpRepository>();
            var translationHttpRepositoryMock = new Mock<ITranslationHttpRepository>();
            var loggerMock = new Mock<ILogger<PokemonService>>();
            var sut = new PokemonService(pokemonHttpRepositoryMock.Object, translationHttpRepositoryMock.Object, loggerMock.Object);
            var token = new CancellationToken();
            pokemonHttpRepositoryMock.Setup(x => x.FindPokemonAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Throws(new Exception());

            //Act
            var result = await sut.SearchPokemonAsync("", false, token);

            //Assert
            result.Should().BeNull();
        }

        [Theory]
        [InlineData("Cave", false, TranslationType.YODA)]
        [InlineData("Forest", true, TranslationType.YODA)]
        [InlineData("Forest", false, TranslationType.SHAKESPEARE)]
        public async Task TranslatePokemonAsync_WithCave_YodaDescription(string habitat, bool isLegendary, TranslationType translationType)
        {
            //Arrange
            var pokemonHttpRepositoryMock = new Mock<IPokemonHttpRepository>();
            var translationHttpRepositoryMock = new Mock<ITranslationHttpRepository>();
            var loggerMock = new Mock<ILogger<PokemonService>>();
            var sut = new PokemonService(pokemonHttpRepositoryMock.Object, translationHttpRepositoryMock.Object, loggerMock.Object);
            var token = new CancellationToken();
            fixture.Customize<PokemonResponse>(c => c.With(p => p.Habitat, habitat).With(p => p.IsLegendary, isLegendary));
            var expPokemon = fixture.Create<PokemonResponse>();
            pokemonHttpRepositoryMock.Setup(x => x.FindPokemonAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(expPokemon);
            translationHttpRepositoryMock.Setup(x => x.TranslateText(It.IsAny<string>(), TranslationType.YODA, It.IsAny<CancellationToken>())).ReturnsAsync(yodaTranslation);
            translationHttpRepositoryMock.Setup(x => x.TranslateText(It.IsAny<string>(), TranslationType.SHAKESPEARE, It.IsAny<CancellationToken>())).ReturnsAsync(shakespeareTranslation);
            var expTranslation = translationType == TranslationType.YODA ? yodaTranslation : shakespeareTranslation;

            //Act
            var result = await sut.SearchPokemonAsync("", true, token);

            //Assert
            result.Should().NotBeNull();
            result.Should().Be(expPokemon);
            result.Description.Should().Be(expTranslation);
        }      
    }
}
