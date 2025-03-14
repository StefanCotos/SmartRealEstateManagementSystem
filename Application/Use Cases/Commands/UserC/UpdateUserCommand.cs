using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.UserC
{
    public class UpdateUserCommand : CreateUserCommand, IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
    }
}
