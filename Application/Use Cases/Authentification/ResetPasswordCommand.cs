using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Authentification
{
    public class ResetPasswordCommand: IRequest<Result<string>>
    {
            public required string Token { get; set; }
            public required string NewPassword { get; set; }
        

    }
}
