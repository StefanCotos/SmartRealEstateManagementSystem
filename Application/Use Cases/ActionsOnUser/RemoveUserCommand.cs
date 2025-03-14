using Domain.Common;
using MediatR;

namespace Application.Use_Cases.ActionsOnUser
{
    public record RemoveUserCommand(Guid Id) : IRequest<Result<Guid>>;

}
