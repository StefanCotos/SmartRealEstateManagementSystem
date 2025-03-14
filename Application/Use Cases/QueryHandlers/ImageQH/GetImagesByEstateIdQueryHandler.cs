using Application.DTO;
using Application.Use_Cases.Queries.Image;
using AutoMapper;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.ImageQH
{
    public class GetImagesByEstateIdQueryHandler : IRequestHandler<GetImagesByEstateIdQuery, List<ImageDto>>
    {
        private readonly IImageRepository repository;
        private readonly IMapper mapper;
        public GetImagesByEstateIdQueryHandler(IImageRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<List<ImageDto>> Handle(GetImagesByEstateIdQuery request, CancellationToken cancellationToken)
        {
            var reviews = await repository.GetAllImagesByEstateIdAsync(request.Id);
            return mapper.Map<List<ImageDto>>(reviews);
        }
    }
}
