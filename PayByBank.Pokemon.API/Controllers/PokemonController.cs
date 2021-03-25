using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using PayByBank.Pokemon.Common.Constants;
using PayByBank.Pokemon.Common.Domain.Pokemon;
using PayByBank.Pokemon.Common.ErrorEnums;
using PayByBank.Pokemon.Common.Interfaces;
using PayByBank.Pokemon.Infrastructure.Monitoring.Errors;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PayByBank.Pokemon.API.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class PokemonController : ControllerBase
    {
        private readonly IPokemonService pokemonService;
        private readonly IActionContextAccessor actionContext;
        private readonly ILogger<PokemonController> logger;

        public PokemonController(IPokemonService pokemonService, IActionContextAccessor actionContext, ILogger<PokemonController> logger)
        {
            this.pokemonService = pokemonService;
            this.actionContext = actionContext;
            this.logger = logger;
        }

        /// <summary>
        /// Searches pokemon by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The pokemon that matches the name</returns>
        /// <response code="200"></response>
        /// <response code="400">Returns a bad request error</response>
        /// <response code="404">Returns a not found error</response>
        /// <response code="500">Returns an internal server  error</response>
        [HttpGet("api/[controller]/{name}")]
        [HttpGet("api/[controller]/translated/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PokemonResponse>> GetPokemon(string name, CancellationToken cancellationToken)
        {

            if (string.IsNullOrEmpty(name))
            {
                logger.CustomLogError(ErrorCategory.BUSINESS, ConstantValues.Error_NoName);
                return BadRequest(ConstantValues.Error_NoName);
            }                

            var translate = actionContext.ActionContext.HttpContext.Request.Path.ToString().IndexOf("translated") > -1;

            PokemonResponse response = null;
            try
            {
                response = await pokemonService.SearchPokemonAsync(name, translate, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.CustomLogError(ErrorCategory.APPLICATION, ex, ConstantValues.Error_InternalError_Controller);
                return StatusCode(StatusCodes.Status500InternalServerError, ConstantValues.Error_InternalError_Client);
            }

            if (response == null)
            {
                logger.CustomLogError(ErrorCategory.APPLICATION, ConstantValues.Error_NotFound);
                return NotFound(string.Format(ConstantValues.Error_NotFound, name));
            }                

            return Ok(response);
        }

    }
}
