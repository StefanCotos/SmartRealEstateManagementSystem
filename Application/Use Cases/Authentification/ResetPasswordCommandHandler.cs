using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.Authentification
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<string>>
    {

        private readonly IUserRepository repository;
        public ResetPasswordCommandHandler(IUserRepository repository) {

            this.repository = repository;

        }
        public async Task<Result<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var result = await repository.ResetPassword(request.Token, request.NewPassword);
            if (!result.IsSuccess)
            {
                return Result<string>.Failure(result.ErrorMessage);
            }
            return Result<string>.Success(result.Data);

        }
    }
}
