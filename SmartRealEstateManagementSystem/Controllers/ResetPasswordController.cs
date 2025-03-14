using Application.Use_Cases.Authentification;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SmartRealEstateManagementSystem.Controllers
{
    [Route("api/reset-password")]
    [ApiController]
    public class ResetPasswordController : ControllerBase
    {
        private readonly IMediator mediator;
        public ResetPasswordController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> SendResetPasswordEmail(SendResetPasswordCommand command)
        {
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result);
        }
        
        [HttpPost("reset-password")]
        public async Task<IActionResult>  ResetPassword(ResetPasswordCommand command)
        {
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result);

        }
    }
}
