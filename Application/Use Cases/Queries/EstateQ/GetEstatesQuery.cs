using Application.DTO;
using MediatR;

namespace Application.Use_Cases.Queries.EstateQ
{
    public class GetEstatesQuery : IRequest<List<EstateDto>>
    {
    }
}
