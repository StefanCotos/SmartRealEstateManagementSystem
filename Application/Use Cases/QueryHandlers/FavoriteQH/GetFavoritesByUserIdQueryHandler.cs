using Application.DTO;
using Application.Use_Cases.Queries.FavoriteQ;
using AutoMapper;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.FavoriteQH
{
    public class GetFavoritesByUserIdQueryHandler : IRequestHandler<GetFavoritesByUserIdQuery, List<FavoriteDto>>
    {
        private readonly IFavoriteRepository repository;
        private readonly IMapper mapper;
        public GetFavoritesByUserIdQueryHandler(IFavoriteRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<List<FavoriteDto>> Handle(GetFavoritesByUserIdQuery request, CancellationToken cancellationToken)
        {
            var favorites = await repository.GetAllFavoritesByUserIdAsync(request.Id);
            return mapper.Map<List<FavoriteDto>>(favorites);
        }
    }
}
