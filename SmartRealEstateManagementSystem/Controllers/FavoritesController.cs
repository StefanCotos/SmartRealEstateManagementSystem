using Application.DTO;
using Application.Use_Cases.Commands.FavoriteC;
using Application.Use_Cases.Queries.FavoriteQ;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartRealEstateManagementSystem.Controllers
{
    [Route("api/favorites")]
    [ApiController]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly IMediator mediator;
        public FavoritesController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> CreateFavorite(CreateFavoriteCommand command)
        {
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return StatusCode(StatusCodes.Status201Created, result.Data);
        }

        [HttpDelete]
        public async Task<ActionResult<Result<Guid>>> DeleteFavorite([FromQuery] Guid userId, [FromQuery] Guid estateId)
        {
            var result = await mediator.Send(new DeleteFavoriteCommand(userId, estateId));
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return StatusCode(StatusCodes.Status200OK, result.Data);
        }

        [HttpGet("users/{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<FavoriteDto>>> GetFavoritesByUserId(Guid id)
        {
            var query = new GetFavoritesByUserIdQuery { Id = id };
            return await mediator.Send(query);
        }
    }
}
