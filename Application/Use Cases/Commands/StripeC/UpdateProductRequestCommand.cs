using MediatR;
using Domain.Common;

namespace Application.Use_Cases.Commands.StripeC
{
    public class UpdateProductRequestCommand : CreateProductRequestCommand, IRequest<Result<Guid>>
    {
        public string? ProductId { get; set; }
    }
}
