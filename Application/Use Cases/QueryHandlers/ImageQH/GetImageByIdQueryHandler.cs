using Application.DTO;
using Application.Use_Cases.Queries.Image;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.ImageQH
{
    public class GetImageByIdQueryHandler : IRequestHandler<GetImageByIdQuery, ImageDto>
    {
        private readonly IGenericEntityRepository<Image> repository;
        private readonly IMapper mapper;

        public GetImageByIdQueryHandler(IGenericEntityRepository<Image> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<ImageDto> Handle(GetImageByIdQuery request, CancellationToken cancellationToken)
        {
            var image = await repository.GetByIdAsync(request.Id);
            return mapper.Map<ImageDto>(image);
        }

    }
}