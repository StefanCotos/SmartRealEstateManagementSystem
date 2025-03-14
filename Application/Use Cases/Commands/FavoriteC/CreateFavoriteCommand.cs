using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.FavoriteC
{
    public class CreateFavoriteCommand : IRequest<Result<Guid>>
    {
        public Guid UserId { get; set; }
        public Guid EstateId { get; set; }
    }
}
