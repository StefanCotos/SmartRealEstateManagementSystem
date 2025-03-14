using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.UserC
{
    public class CreateUserCommand : IRequest<Result<Guid>>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}
