using Application.Use_Cases.Commands.FavoriteC;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.FavoriteCH
{
    public class DeleteFavoriteCommandHandler : IRequestHandler<DeleteFavoriteCommand, Result<Guid>>
    {
        private readonly IFavoriteRepository repository;

        public DeleteFavoriteCommandHandler(IFavoriteRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Result<Guid>> Handle(DeleteFavoriteCommand request, CancellationToken cancellationToken)
        {
            var result = await repository.DeleteAsync(request.UserId, request.EstateId);
            if (result.IsSuccess)
            {
                return Result<Guid>.Success(result.Data);
            }
            return Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}
