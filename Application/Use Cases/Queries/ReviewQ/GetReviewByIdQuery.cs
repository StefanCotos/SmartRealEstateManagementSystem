using Application.DTO;
using MediatR;

namespace Application.Use_Cases.Queries.ReviewQ
{
    public class GetReviewByIdQuery : IRequest<ReviewDto>
    {
        public Guid Id { get; set; }
    }
}
