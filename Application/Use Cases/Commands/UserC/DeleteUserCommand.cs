using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.UserC
{
    public record DeleteUserCommand(Guid Id): IRequest<Result<Guid>>;
}
