using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.StripeC
{
    public class CreateProductRequestCommand : IRequest<Result<Guid>>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal UnitAmount { get; set; }
    }
}
