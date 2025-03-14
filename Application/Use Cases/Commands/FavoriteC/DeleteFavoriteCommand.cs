using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.FavoriteC
{
    public record DeleteFavoriteCommand(Guid UserId, Guid EstateId) : IRequest<Result<Guid>>;
}
