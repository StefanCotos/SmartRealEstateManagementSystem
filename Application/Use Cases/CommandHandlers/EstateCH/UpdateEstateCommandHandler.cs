using Application.Use_Cases.Commands.EstateC;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.EstateCH
{
    public class UpdateEstateCommandHandler : IRequestHandler<UpdateEstateCommand, Result<Guid>>
    {
        private readonly IGenericEntityRepository<Estate> repository;
        private readonly IMapper mapper;

        public UpdateEstateCommandHandler(IGenericEntityRepository<Estate> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(UpdateEstateCommand request, CancellationToken cancellationToken)
        {
            var estate = mapper.Map<Estate>(request);
            var result = await repository.UpdateAsync(estate);
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
