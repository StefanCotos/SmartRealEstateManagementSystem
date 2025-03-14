using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Authentification
{
    public class SendResetPasswordCommand : IRequest<Result<string>>
    {
        public required string Email { get; set; }

    }
}
