using Application.DTO;
using MediatR;

namespace Application.Use_Cases.Queries.Image
{
    public class GetImagesQuery : IRequest<List<ImageDto>>
    {
    }
}