using Application.Use_Cases.Commands.ImageC;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.ImageCH
{
    public class DeleteImageCommandHandler : IRequestHandler<DeleteImageCommand, Result<Guid>>
    {
        private readonly IGenericEntityRepository<Image> repository;

        public DeleteImageCommandHandler(IGenericEntityRepository<Image> repository)
        {
            this.repository = repository;
        }

        public async Task<Result<Guid>> Handle(DeleteImageCommand request, CancellationToken cancellationToken)
        {
            var result = await repository.DeleteAsync(request.Id);
            if (result.IsSuccess)
            {
                return Result<Guid>.Success(result.Data);
            }
            return Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}
