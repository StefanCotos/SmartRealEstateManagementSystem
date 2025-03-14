using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.Authentification
{
    public class SendResetPasswordCommandHandler : IRequestHandler<SendResetPasswordCommand, Result<string>>
    {
        private readonly IUserRepository userRepository;
        public SendResetPasswordCommandHandler(IUserRepository userrepository)
        {
            this.userRepository = userrepository;
        }
        public async Task<Result<string>> Handle(SendResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var result= await userRepository.SendResetPassword(request.Email);
            if (!result.IsSuccess)
            {
                return Result<string>.Failure(result.ErrorMessage);
            }

            return Result<string>.Success(result.Data);


        }
    }
}
