using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.ImageC
{
    public class CreateImageCommand : IRequest<Result<Guid>>
    {
        public Guid EstateId { get; set; }
        public required string Extension { get; set; }
        public Guid Id { get; set; }
    }
}
