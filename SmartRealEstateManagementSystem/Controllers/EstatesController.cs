using Application.DTO;
using Application.Use_Cases.Commands.EstateC;
using Application.Use_Cases.Queries.EstateQ;
using Application.Utils;
using Application.Utils.FilterStrategy;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartRealEstateManagementSystem.Controllers
{
    [Route("api/estates")]
    [ApiController]
    [Authorize]
    public class EstatesController : ControllerBase
    {
        private readonly IMediator mediator;
        public EstatesController(IMediator mediator)
        {
            this.mediator = mediator;
        }


        [HttpPost]
       
        public async Task<ActionResult<EstateDto>> CreateEstate(CreateEstateCommand command)
        {
            Console.WriteLine(command);

            var result = await mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return StatusCode(StatusCodes.Status201Created, result.Data);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<EstateDto>>> GetEstates()
        {
            return await mediator.Send(new GetEstatesQuery());
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<EstateDto>> GetEstateById(Guid id)
        {
            var query = new GetEstateByIdQuery { Id = id };
            var result = await mediator.Send(query);
            if (result == null)
            {
                return BadRequest("Estate not found");
            }
            return Ok(result);
        }

        [HttpGet("users/{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EstateDto>>> GetEstatesByUserId(Guid id)
        {
            var query = new GetEstatesByUserIdQuery { Id = id };
            return await mediator.Send(query);
        }

        [HttpGet("buyers/{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EstateDto>>> GetEstatesByBuyerId(Guid id)
        {
            var query = new GetEstatesByBuyerIdQuery { Id = id };
            return await mediator.Send(query);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Result<Guid>>> UpdateEstate(Guid id, UpdateEstateCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("The id should be identical with command.Id");
            }
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return StatusCode(StatusCodes.Status200OK, result.Data);
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Result<Guid>>> DeleteEstate(Guid id)
        {
            var result = await mediator.Send(new DeleteEstateCommand(id));
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return StatusCode(StatusCodes.Status200OK, result.Data);
        }

        [HttpGet("filter/paginated")]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResult<EstateDto>>> GetFilteredPaginatedEstate([FromQuery] EstateFilterParams filterParams)
        {
            if (filterParams.Price < 0 || filterParams.Bedrooms < 0 || filterParams.Bathrooms < 0 || filterParams.LandSize < 0 || filterParams.HouseSize < 0)
            {
                return BadRequest("Price or Size cannot be negative");
            }

            var query = new GetEstatesPaginationByFilterQuery
            {
                Name = filterParams.Name,
                Page = filterParams.Page,
                PageSize = filterParams.PageSize,
                Price = filterParams.Price,
                Bedrooms = filterParams.Bedrooms,
                Bathrooms = filterParams.Bathrooms,
                LandSize = filterParams.LandSize,
                Street = filterParams.Street,
                City = filterParams.City,
                State = filterParams.State,
                HouseSize = filterParams.HouseSize,
            };
            var result = await mediator.Send(query);
            return Ok(result);
        }
    }
}

