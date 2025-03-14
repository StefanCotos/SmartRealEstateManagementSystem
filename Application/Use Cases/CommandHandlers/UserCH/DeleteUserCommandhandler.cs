using Application.Use_Cases.Commands.UserC;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.UserCH
{
    public class DeleteUserCommandhandler : IRequestHandler<DeleteUserCommand, Result<Guid>>
    {
        private readonly IGenericEntityRepository<User> repository;
        public DeleteUserCommandhandler(IGenericEntityRepository<User> repository)
        {
            this.repository = repository;
        }

        public async Task<Result<Guid>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var result = await repository.DeleteAsync(request.Id);

            if (result.IsSuccess)
            {
                return Result<Guid>.Success(result.Data);
            }
            return Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}
