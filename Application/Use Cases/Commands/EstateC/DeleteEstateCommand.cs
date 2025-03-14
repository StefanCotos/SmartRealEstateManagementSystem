using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.EstateC
{
    public record DeleteEstateCommand(Guid Id) : IRequest<Result<Guid>>;
}
