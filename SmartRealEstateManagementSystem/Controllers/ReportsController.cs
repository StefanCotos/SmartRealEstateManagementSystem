using Application.DTO;
using Application.Use_Cases.Commands.ReportC;
using Application.Use_Cases.Queries.Report;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartRealEstateManagementSystem.Controllers
{
    [Route("api/reports")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator mediator;
        public ReportsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Result<Guid>>> CreateReport(CreateReportCommand command)
        {
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return StatusCode(StatusCodes.Status201Created, result.Data);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<ReportDto>>> GetReports()
        {
            return await mediator.Send(new GetReportsQuery());
        }
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<ReportDto>> GetReportById(Guid id)
        {
            var query = new GetReportByIdQuery { Id = id };
            var result = await mediator.Send(query);
            if (result == null)
            {
                return BadRequest("Report not found");
            }
            return Ok(result);
        }
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Result<Guid>>> DeleteReport(Guid id)
        {
            var result = await mediator.Send(new DeleteReportCommand(id));
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }
    }
}
