using Application.DTO;
using Application.Use_Cases.Queries.EstateQ;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.EstateQH
{
    public class GetEstateByIdQueryHandler : IRequestHandler<GetEstateByIdQuery, EstateDto>
    {
        private readonly IGenericEntityRepository<Estate> repository;
        private readonly IMapper mapper;

        public GetEstateByIdQueryHandler(IGenericEntityRepository<Estate> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<EstateDto> Handle(GetEstateByIdQuery request, CancellationToken cancellationToken)
        {
            var estate = await repository.GetByIdAsync(request.Id);
            return mapper.Map<EstateDto>(estate);

        }
    }
}
