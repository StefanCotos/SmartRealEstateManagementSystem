using Application.DTO;
using MediatR;

namespace Application.Use_Cases.Queries.EstateQ
{
    public class GetEstatesByBuyerIdQuery : IRequest<List<EstateDto>>
    {
        public Guid Id { get; set; }
    }
}
