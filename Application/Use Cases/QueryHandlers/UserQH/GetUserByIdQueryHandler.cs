using Application.DTO;
using Application.Use_Cases.Queries.UserQ;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.UserQH
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IGenericEntityRepository<User> repository;
        private readonly IMapper mapper;
        public GetUserByIdQueryHandler(IGenericEntityRepository<User> repository,IMapper mapper)
        {
           this.repository = repository;
           this.mapper = mapper;
        }

        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await repository.GetByIdAsync(request.Id);
            return mapper.Map<UserDto>(user);
        }
    }
}
