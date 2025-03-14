using Application.DTO;
using Application.Use_Cases.Queries.EstateQ;
using AutoMapper;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.EstateQH
{
    public class GetEstatesByUserIdQueryHandler : IRequestHandler<GetEstatesByUserIdQuery, List<EstateDto>>
    {
        private readonly IEstateRepository repository;
        private readonly IMapper mapper;
        public GetEstatesByUserIdQueryHandler(IEstateRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<List<EstateDto>> Handle(GetEstatesByUserIdQuery request, CancellationToken cancellationToken)
        {
            var estates = await repository.GetAllEstatesByUserIdAsync(request.Id);
            return mapper.Map<List<EstateDto>>(estates);
        }
    }
}
