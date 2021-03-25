using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using PayByBank.Pokemon.Common.Constants;
using PayByBank.Pokemon.Common.Domain.Pokemon;
using PayByBank.Pokemon.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace PayByBank.Pokemon.API.Controllers
{
    [ApiController]
    [Produces("application/json")]
    //[Route("api/[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly IPokemonService pokemonService;
        private readonly IActionContextAccessor actionContext;

        public PokemonController(IPokemonService pokemonService, IActionContextAccessor actionContext)
        {
            this.pokemonService = pokemonService;
            this.actionContext = actionContext;
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
        [HttpGet("api/[controller]/{name}")]
        [HttpGet("api/[controller]/translated/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PokemonResponse>> GetPokemon(string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest(Constants.Error_NoName);

            var translate = actionContext.ActionContext.HttpContext.Request.Path.ToString().IndexOf("translated") > -1;

            PokemonResponse response = null;
            try
            {
                response = await pokemonService.SearchPokemonAsync(name, translate, cancellationToken);
            }
            catch
            {
                //log errors
            }            

            if (response == null)
                return NotFound(string.Format(Constants.Error_NotFound, name));

            return Ok(response);
        }

    }
}
