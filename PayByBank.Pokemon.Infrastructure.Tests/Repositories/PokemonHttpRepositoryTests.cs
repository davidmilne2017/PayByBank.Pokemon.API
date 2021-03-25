using FluentAssertions;
using Moq;
using AutoFixture;
using Moq.Protected;
using PayByBank.Pokemon.Infrastructure.Repositories;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using PayByBank.Pokemon.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using PayByBank.Pokemon.Common.Constants;
using PayByBank.Pokemon.Common.Domain.Pokemon;
using Microsoft.Extensions.Logging;

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

            var configurationSectionMock = new Mock<IConfigurationSection>();
            configurationSectionMock.Setup(x => x.Value).Returns(ConstantValues.PokemonApi);
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x.GetSection(It.Is<string>(k => k == ConstantValues.PokemonApi))).Returns(configurationSectionMock.Object);

            var pokemonReturn = Common.Resources.Pokemon.ResourceManager.GetString("pikachu");
            var resource = "https://test.com/";
            var pokemonName = "pikachu";
            var expPokemon = fixture.Create<PokemonResponse>();

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
                BaseAddress = new Uri(resource),
            };

            var loggerMock = new Mock<ILogger<PokemonHttpRepository>>();
            var sut = new PokemonHttpRepository(httpClientFactoryMock.Object, pokemonConverterAdapterMock.Object, configurationMock.Object, loggerMock.Object);
            var token = new CancellationToken();
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            pokemonConverterAdapterMock.Setup(x => x.ConvertPokemon(It.IsAny<PokemonApiReturn>())).Returns(expPokemon);

            //Act
            var result = await sut.FindPokemonAsync(pokemonName, token);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<PokemonResponse>();
            result.Should().Be(expPokemon);
            
            var expectedUri = new Uri($"{resource}{ConstantValues.PokemonApi}/{pokemonName}");

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

            var configurationSectionMock = new Mock<IConfigurationSection>();
            configurationSectionMock.Setup(x => x.Value).Returns(ConstantValues.PokemonApi);
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x.GetSection(It.Is<string>(k => k == ConstantValues.PokemonApi))).Returns(configurationSectionMock.Object);

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

            var loggerMock = new Mock<ILogger<PokemonHttpRepository>>();
            var sut = new PokemonHttpRepository(httpClientFactoryMock.Object, pokemonConverterAdapterMock.Object, configurationMock.Object, loggerMock.Object);
            var token = new CancellationToken();
            httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            //Act
            var result = await sut.FindPokemonAsync(resource, token);

            //Assert
            result.Should().BeNull();
        }

    }
}
