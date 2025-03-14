using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.ImageC
{
    public class UpdateImageCommand : IRequest<Result<Guid>>
    {
        public Guid Id { get; set; }
        public Guid EstateId { get; set; }
        public required string Extension { get; set; }
    }
}
