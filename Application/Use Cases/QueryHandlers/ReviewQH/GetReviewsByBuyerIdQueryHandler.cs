using Application.DTO;
using Application.Use_Cases.Queries.ReviewQ;
using AutoMapper;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.ReviewQH
{
    public class GetReviewsByBuyerIdQueryHandler : IRequestHandler<GetReviewsByBuyerIdQuery, List<ReviewDto>>
    {
        private readonly IReviewRepository repository;
        private readonly IMapper mapper;
        public GetReviewsByBuyerIdQueryHandler(IReviewRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<List<ReviewDto>> Handle(GetReviewsByBuyerIdQuery request, CancellationToken cancellationToken)
        {
            var reviews = await repository.GetAllBuyerReviewAsync(request.Id);
            return mapper.Map<List<ReviewDto>>(reviews);
        }
    }
}
