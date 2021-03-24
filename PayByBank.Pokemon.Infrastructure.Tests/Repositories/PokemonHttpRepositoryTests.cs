using FluentAssertions;
using Moq;
using Moq.Protected;
using System.Linq;
using PayByBank.Pokemon.Common.Domain;
using PayByBank.Pokemon.Infrastructure.Repositories;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using AutoFixture;
using PayByBank.Pokemon.Common.Interfaces;

namespace PayByBank.Pokemon.Infrastructure.Tests.Repositories
{
    public class PokemonHttpRepositoryTests
    {
        private readonly Fixture fixture;

        public PokemonHttpRepositoryTests()
        {
            this.fixture = new Fixture();
        }

        [Fact]
        public async Task FindPokemonAsync_CorrectResponse_ReturnsPokemon()
        {
            //Arrange
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var pokemonConverterAdapterMock = new Mock<IPokemonConverterAdapter>();
            var pokemonReturn = Common.Resources.Pokemon.ResourceManager.GetString("pikachu");
            var baseAddress = "http://test.com/";
            var resource = "api/test";

            var expName = "Pikachu";
            var expHabitat = "forest";
            var expIsLegendary = false;
            var expDescription = "Some description";
            var expPokemon = new PokemonResponse()
            {
                Name = expName,
                Habitat = expHabitat,
                IsLegendary = expIsLegendary,
                Description = expDescription
            };

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(pokemonReturn),
                })
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var sut = new PokemonHttpRepository(httpClientFactoryMock.Object, pokemonConverterAdapterMock.Object);
            var token = new CancellationToken();
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            pokemonConverterAdapterMock.Setup(x => x.ConvertPokemon(It.IsAny<PokemonApiReturn>())).Returns(expPokemon);

            //Act
            var result = await sut.FindPokemonAsync(resource, token);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<PokemonResponse>();
            result.Should().Be(expPokemon);
            
            var expectedUri = new Uri($"{baseAddress}{resource}");

            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1), 
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Get  
                  && req.RequestUri == expectedUri 
               ),
               ItExpr.IsAny<CancellationToken>()
            );

        }

        [Fact]
        public async Task FindPokemonAsync_InCorrectResponse_ReturnsNull()
        {
            //Arrange
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var pokemonConverterAdapterMock = new Mock<IPokemonConverterAdapter>();
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var baseAddress = "http://test.com/";
            var resource = "api/test";

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Throws(new InvalidOperationException())
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(baseAddress),
            };

            var sut = new PokemonHttpRepository(httpClientFactoryMock.Object, pokemonConverterAdapterMock.Object);
            var token = new CancellationToken();
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            //Act
            var result = await sut.FindPokemonAsync(resource, token);

            //Assert
            result.Should().BeNull();
        }

    }
}
