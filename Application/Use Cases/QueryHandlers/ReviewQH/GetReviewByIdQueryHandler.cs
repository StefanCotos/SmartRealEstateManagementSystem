using Application.DTO;
using Application.Use_Cases.Queries.ReviewQ;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.ReviewQH
{
    public class GetReviewByIdQueryHandler : IRequestHandler<GetReviewByIdQuery, ReviewDto>
    {
        private readonly IGenericEntityRepository<Review> repository;
        private readonly IMapper mapper;
        public GetReviewByIdQueryHandler(IGenericEntityRepository<Review> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<ReviewDto> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
        {
            var reviewUser = await repository.GetByIdAsync(request.Id);
            return mapper.Map<ReviewDto>(reviewUser);
        }
    }
}
