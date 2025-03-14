using Application.DTO;
using MediatR;

namespace Application.Use_Cases.Queries.Image
{
    public class GetImageByIdQuery : IRequest<ImageDto>
    {
        public Guid Id { get; set; }
    }
}