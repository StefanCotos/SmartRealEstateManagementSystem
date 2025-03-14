using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.StripeC
{
    public class CreatePayRequestCommand : IRequest<Result<string>>
    {
        public string? defaultPriceId { get; set; }
    }

}