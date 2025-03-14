using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.ActionsOnUser
{
    public class RemoveUserCommandHandler : IRequestHandler<RemoveUserCommand, Result<Guid>>
    {
        private readonly IUserRepository repository;
        public RemoveUserCommandHandler(IUserRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Result<Guid>> Handle(RemoveUserCommand request, CancellationToken cancellationToken)
        {
            var result = await repository.RemoveAsync(request.Id);

            if (result.IsSuccess)
            {
                return Result<Guid>.Success(result.Data);
            }
            return Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}
