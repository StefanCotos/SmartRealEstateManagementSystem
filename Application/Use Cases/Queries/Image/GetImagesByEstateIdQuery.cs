using Application.DTO;
using MediatR;

namespace Application.Use_Cases.Queries.Image
{
    public class GetImagesByEstateIdQuery : IRequest<List<ImageDto>>
    {
        public Guid Id { get; set; }
    }
}
