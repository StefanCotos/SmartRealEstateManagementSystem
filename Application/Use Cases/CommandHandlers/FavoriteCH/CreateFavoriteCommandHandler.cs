using Application.Use_Cases.Commands.FavoriteC;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.FavoriteCH
{
    public class CreateFavoriteCommandHandler : IRequestHandler<CreateFavoriteCommand, Result<Guid>>
    {
        private readonly IGenericEntityRepository<Favorite> repository;
        private readonly IMapper mapper;

        public CreateFavoriteCommandHandler(IGenericEntityRepository<Favorite> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<Result<Guid>> Handle(CreateFavoriteCommand request, CancellationToken cancellationToken)
        {
            var favorite = mapper.Map<Favorite>(request);
            var result = await repository.AddAsync(favorite);
            if (result.IsSuccess)
            {
                return Result<Guid>.Success(result.Data);
            }
            return Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}
