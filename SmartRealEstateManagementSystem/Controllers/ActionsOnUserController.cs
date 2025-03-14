using Application.Use_Cases.ActionsOnUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace SmartRealEstateManagementSystem.Controllers
{
    [Route("api/actions-on-user")]
    [ApiController]
    [Authorize]
    public class ActionsOnUserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ActionsOnUserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> RemoveUser(Guid id)
        {
            var result = await _mediator.Send(new RemoveUserCommand(id));
            if (!result.IsSuccess)
            {
                // Returnează BadRequest cu mesaj de eroare
                return BadRequest(result.ErrorMessage);
            }
            return StatusCode(StatusCodes.Status200OK, result.Data);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> EditUser(Guid id, EditUserCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("The id should be identical with command.Id");
            }
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return StatusCode(StatusCodes.Status200OK, result.Data);
        }

        [HttpPost("check-password")]
        public async Task<IActionResult> CheckPassword(CheckPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return StatusCode(StatusCodes.Status200OK, result.Data);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return StatusCode(StatusCodes.Status200OK, result.Data);
        }
    }
}
