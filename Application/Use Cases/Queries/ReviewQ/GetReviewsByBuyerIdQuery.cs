using Application.DTO;
using MediatR;

namespace Application.Use_Cases.Queries.ReviewQ
{
    public class GetReviewsByBuyerIdQuery : IRequest<List<ReviewDto>>
    {
        public Guid Id { get; set; }
    }
}
