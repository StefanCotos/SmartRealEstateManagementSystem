using Domain.Common;
using MediatR;

namespace Application.Use_Cases.ActionsOnUser
{
    public class SendContactFormCommand : IRequest<Result<string>>
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Subject { get; set; }
        public required string Message { get; set; }
    }
}
