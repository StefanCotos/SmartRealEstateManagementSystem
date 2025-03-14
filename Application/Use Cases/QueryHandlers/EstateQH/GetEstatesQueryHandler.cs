using Application.DTO;
using Application.Use_Cases.Queries.EstateQ;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.EstateQH
{
    public class GetEstatesQueryHandler : IRequestHandler<GetEstatesQuery, List<EstateDto>>
    {
        private readonly IGenericEntityRepository<Estate> repository;
        private readonly IMapper mapper;

        public GetEstatesQueryHandler(IGenericEntityRepository<Estate> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<List<EstateDto>> Handle(GetEstatesQuery request, CancellationToken cancellationToken)
        {
            var estates = await repository.GetAllAsync();
            return mapper.Map<List<EstateDto>>(estates);
        }


    }
}
