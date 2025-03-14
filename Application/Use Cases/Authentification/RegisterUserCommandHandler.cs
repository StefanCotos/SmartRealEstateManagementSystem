using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.Authentification
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<Guid>>
    {
        private readonly IUserRepository repository;

        public RegisterUserCommandHandler(IUserRepository repository) => this.repository = repository;

        public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Email = request.Email,
                UserName = request.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            var result = await repository.Register(user, cancellationToken);
            if (!result.IsSuccess)
            {
                return Result<Guid>.Failure(result.ErrorMessage);
            }
            user.Id = result.Data; // Ensure user.Id is set to the value returned by the repository
            return Result<Guid>.Success(user.Id);
        }
    }
}