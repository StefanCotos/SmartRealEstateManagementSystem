using Application.DTO;
using Application.Use_Cases.Queries.EstateQ;
using AutoMapper;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.EstateQH
{
    public class GetEstatesByBuyerIdQueryHandler : IRequestHandler<GetEstatesByBuyerIdQuery, List<EstateDto>>
    {
        private readonly IEstateRepository repository;
        private readonly IMapper mapper;
        public GetEstatesByBuyerIdQueryHandler(IEstateRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<List<EstateDto>> Handle(GetEstatesByBuyerIdQuery request, CancellationToken cancellationToken)
        {
            var estates = await repository.GetAllEstatesByBuyerIdAsync(request.Id);
            return mapper.Map<List<EstateDto>>(estates);
        }
    }
}
