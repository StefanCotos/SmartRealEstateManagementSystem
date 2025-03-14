using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.ActionsOnUser
{
    public class SendContactFormCommandHandler : IRequestHandler<SendContactFormCommand, Result<string>>
    {
        private readonly IUserRepository userRepository;

        public SendContactFormCommandHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        public async Task<Result<string>> Handle(SendContactFormCommand request, CancellationToken cancellationToken)
        {
            var result = await userRepository.SendContactForm(request.Name, request.Email, request.Subject, request.Message);
            if (!result.IsSuccess)
            {
                return Result<string>.Failure(result.ErrorMessage);
            }

            return Result<string>.Success(result.Data);

        }
    }
}
