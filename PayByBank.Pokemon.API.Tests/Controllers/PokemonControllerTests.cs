using Xunit;
using Moq;
using FluentAssertions;
using System.Threading.Tasks;
using PayByBank.Pokemon.Common.Interfaces;
using PayByBank.Pokemon.API.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using PayByBank.Pokemon.Common.Constants;
using Microsoft.AspNetCore.Http;
using PayByBank.Pokemon.Common.Domain.Pokemon;
using AutoFixture;
using Microsoft.Extensions.Logging;

namespace PayByBank.Pokemon.API.Tests.Controllers
{
    public class PokemonControllerTests
    {

        private const string pokemonName = "pikachu";

        [Fact]
        public async Task GetPokemon_WithNoName_Returns_BadRequest()
        {
            //Arrange
            var pokemonServiceMock = new Mock<IPokemonService>();
            var actionContextAccessorMock = new Mock<IActionContextAccessor>();
            var loggerMock = new Mock<ILogger<PokemonController>>();
            var sut = new PokemonController(pokemonServiceMock.Object, actionContextAccessorMock.Object, loggerMock.Object);
            var cancellationToken = new CancellationToken();

            //Act
            var response = await sut.GetPokemon("", cancellationToken);

            //Assert
            var result = response.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            result.Value.Should().Be(ConstantValues.Error_NoName);
        }

        [Fact]
        public async Task GetPokemon_WithException_Returns_InternalServerError()
        {
            //Arrange
            var pokemonServiceMock = new Mock<IPokemonService>();
            pokemonServiceMock.Setup(x => x.SearchPokemonAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Throws(new System.Exception());

            var actionContextAccessorMock = new Mock<IActionContextAccessor>();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/somepath";
            var actionContext = new ActionContext();
            actionContext.HttpContext = httpContext;
            actionContextAccessorMock.Setup(x => x.ActionContext).Returns(actionContext);

            var loggerMock = new Mock<ILogger<PokemonController>>();
            var sut = new PokemonController(pokemonServiceMock.Object, actionContextAccessorMock.Object, loggerMock.Object);
            var cancellationToken = new CancellationToken();

            //Act
            var response = await sut.GetPokemon(pokemonName, cancellationToken);

            //Assert
            var result = response.Result.Should().BeOfType<ObjectResult>().Subject;
            result.Value.Should().Be(ConstantValues.Error_InternalError_Client);
        }

        [Fact]
        public async Task GetPokemon_WithNull_Returns_NotFound()
        {
            //Arrange
            var pokemonServiceMock = new Mock<IPokemonService>();
            pokemonServiceMock.Setup(x => x.SearchPokemonAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((PokemonResponse)null);

            var actionContextAccessorMock = new Mock<IActionContextAccessor>();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/somepath";
            var actionContext = new ActionContext();
            actionContext.HttpContext = httpContext;
            actionContextAccessorMock.Setup(x => x.ActionContext).Returns(actionContext);

            var loggerMock = new Mock<ILogger<PokemonController>>();
            var sut = new PokemonController(pokemonServiceMock.Object, actionContextAccessorMock.Object, loggerMock.Object);
            var cancellationToken = new CancellationToken();

            //Act
            var response = await sut.GetPokemon(pokemonName, cancellationToken);

            //Assert
            var result = response.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
            result.Value.Should().Be(string.Format(ConstantValues.Error_NotFound, pokemonName));
        }

        [Fact]
        public async Task GetPokemon_WithResponse_Returns_Ok()
        {
            //Arrange
            var pokemonServiceMock = new Mock<IPokemonService>();
            var fixture = new Fixture();
            fixture.Customize<PokemonResponse>(c => c.With(p => p.Name, pokemonName));
            var pokemonResponse = fixture.Create<PokemonResponse>();
            pokemonServiceMock.Setup(x => x.SearchPokemonAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pokemonResponse);

            var actionContextAccessorMock = new Mock<IActionContextAccessor>();
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/somepath";
            var actionContext = new ActionContext();
            actionContext.HttpContext = httpContext;
            actionContextAccessorMock.Setup(x => x.ActionContext).Returns(actionContext);

            var loggerMock = new Mock<ILogger<PokemonController>>();
            var sut = new PokemonController(pokemonServiceMock.Object, actionContextAccessorMock.Object, loggerMock.Object);
            var cancellationToken = new CancellationToken();

            //Act
            var response = await sut.GetPokemon(pokemonName, cancellationToken);

            //Assert
            var result = response.Result.Should().BeOfType<OkObjectResult>().Subject;
            var value = result.Value.Should().BeOfType<PokemonResponse>().Subject;
            value.Should().Be(pokemonResponse);
        }
    }
}
