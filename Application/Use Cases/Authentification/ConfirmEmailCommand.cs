using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Authentification
{
    public class ConfirmEmailCommand : IRequest<Result<string>>
    {
        public required string Token { get; set; }

    }
}
