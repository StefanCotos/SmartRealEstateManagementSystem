using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Authentification
{
    public class RegisterUserCommand : IRequest<Result<Guid>>
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Username { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

    }
}