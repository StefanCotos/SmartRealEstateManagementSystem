using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.Authentification
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result<string>>
    {
        private readonly IUserRepository userRepository;
        public ConfirmEmailCommandHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        public async Task<Result<string>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var result = await userRepository.ConfirmEmail(request.Token);
            if (!result.IsSuccess)
            {
                return Result<string>.Failure(result.ErrorMessage);
            }
            return Result<string>.Success(result.Data);
        }
    }
}
