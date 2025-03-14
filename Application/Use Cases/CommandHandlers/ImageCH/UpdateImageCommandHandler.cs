using Application.Use_Cases.Commands.ImageC;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;


namespace Application.Use_Cases.CommandHandlers.ImageCH
{
    public class UpdateImageCommandHandler : IRequestHandler<UpdateImageCommand, Result<Guid>>
    {
        private readonly IGenericEntityRepository<Image> repository;
        private readonly IMapper mapper;

        public UpdateImageCommandHandler(IGenericEntityRepository<Image> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(UpdateImageCommand request, CancellationToken cancellationToken)
        {
            var image = mapper.Map<Image>(request);
            var result = await repository.UpdateAsync(image);
            if (result == null)
            {
                return Result<Guid>.Failure("Update operation failed.");
            }
            if (result.IsSuccess)
            {
                return Result<Guid>.Success(result.Data);
            }
            return Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}
