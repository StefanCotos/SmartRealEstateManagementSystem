using Application.Use_Cases.Commands.UserC;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.UserCH
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<Guid>>
    {
        private readonly IGenericEntityRepository<User> repository;
        private readonly IMapper mapper;

        public UpdateUserCommandHandler(IGenericEntityRepository<User> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = mapper.Map<User>(request);
            var result = await repository.UpdateAsync(user);
            if (result == null)
            {
                return Result<Guid>.Failure("Update operation failed.");
            }
            if (result.IsSuccess)
            {
                return Result<Guid>.Success(result.Data);
            }
            return Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}
