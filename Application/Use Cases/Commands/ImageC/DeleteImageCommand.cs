using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.ImageC
{
    public record DeleteImageCommand(Guid Id) : IRequest<Result<Guid>>;
}
