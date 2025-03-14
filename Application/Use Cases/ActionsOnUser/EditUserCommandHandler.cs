using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.ActionsOnUser
{
    public class EditUserCommandHandler : IRequestHandler<EditUserCommand, Result<String>>
    {
        private readonly IUserRepository repository;
        private readonly IMapper mapper;

        public EditUserCommandHandler(IUserRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<String>> Handle(EditUserCommand request, CancellationToken cancellationToken)
        {
            var user = mapper.Map<User>(request);
            var result = await repository.EditAsync(user);
            if (result == null)
            {
                return Result<String>.Failure("Update operation failed.");
            }
            if (result.IsSuccess)
            {
                return Result<String>.Success(result.Data);
            }
            return Result<String>.Failure(result.ErrorMessage);
        }
    }
}
