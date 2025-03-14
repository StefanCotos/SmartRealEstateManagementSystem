using Application.DTO;
using Application.Use_Cases.Queries.UserQ;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.UserQH
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
    {
        private readonly IGenericEntityRepository<User> repository;
        private readonly IMapper mapper;

        public GetUsersQueryHandler(IGenericEntityRepository<User> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var Users = await repository.GetAllAsync();
            return mapper.Map<List<UserDto>>(Users);
        }
    }
}
