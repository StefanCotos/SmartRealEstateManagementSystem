using Application.DTO;
using MediatR;

namespace Application.Use_Cases.Queries.ReviewQ
{
    public class GetReviewsBySellerIdQuery: IRequest<List<ReviewDto>>
    {
        public Guid Id { get; set; }
    }
}
