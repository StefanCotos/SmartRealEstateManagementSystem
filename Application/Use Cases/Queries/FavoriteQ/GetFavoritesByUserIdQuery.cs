using Application.DTO;
using MediatR;

namespace Application.Use_Cases.Queries.FavoriteQ
{
    public class GetFavoritesByUserIdQuery : IRequest<List<FavoriteDto>>
    {
        public Guid Id { get; set; }
    }
}
