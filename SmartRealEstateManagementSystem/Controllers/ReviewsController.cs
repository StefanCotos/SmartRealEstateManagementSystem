using Application.DTO;
using Application.Use_Cases.Commands.ReviewUserC;
using Application.Use_Cases.Queries.ReviewQ;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartRealEstateManagementSystem.Controllers
{
    [Route("api/review-users")]
    [ApiController]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly IMediator mediator;
        public ReviewsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> CreateReviewsUser(CreateReviewUserCommand command)
        {
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return StatusCode(StatusCodes.Status201Created, result.Data);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<ReviewDto>> GetReviewUserById(Guid id)
        {
            var query = new GetReviewByIdQuery { Id = id };
            var result = await mediator.Send(query);
            if (result == null)
            {
                return BadRequest("ReviewUser not found");
            }
            return Ok(result);
        }

        [HttpGet("buyers/{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ReviewDto>>> GetReviewByBuyerId(Guid id)
        {
            var query = new GetReviewsByBuyerIdQuery { Id = id };
            return await mediator.Send(query);
        }

        [HttpGet("sellers/{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ReviewDto>>> GetReviewBySellerId(Guid id)
        {
            var query = new GetReviewsBySellerIdQuery { Id = id };
            return await mediator.Send(query);
        }
    }
}
