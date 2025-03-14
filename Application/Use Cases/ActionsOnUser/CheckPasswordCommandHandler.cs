using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.ActionsOnUser
{
    public class CheckPasswordCommandHandler : IRequestHandler<CheckPasswordCommand, Result<bool>>
    {
        private readonly IUserRepository userRepository;

        public CheckPasswordCommandHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<Result<bool>> Handle(CheckPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Email = request.Email,
                Password = request.Password
            };
            var result = await userRepository.CheckPassword(user);
            if (!result.IsSuccess)
            {
                return Result<bool>.Failure(result.ErrorMessage);
            }
            return Result<bool>.Success(true);
        }
    }
}
