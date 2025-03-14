using Application.DTO;
using MediatR;

namespace Application.Use_Cases.Queries.EstateQ
{
    public class GetEstateByIdQuery : IRequest<EstateDto>
    {
        public Guid Id { get; set; }
    }
}
