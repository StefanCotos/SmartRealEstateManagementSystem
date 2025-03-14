using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;
using Application.Use_Cases.Commands.EstateC;

namespace Application.Use_Cases.CommandHandlers.EstateCH
{
    public class CreateEstateCommandHandler : IRequestHandler<CreateEstateCommand, Result<Guid>>
    {
        private readonly IGenericEntityRepository<Estate> repository;
        private readonly IMapper mapper;

        public CreateEstateCommandHandler(IGenericEntityRepository<Estate> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<Result<Guid>> Handle(CreateEstateCommand request, CancellationToken cancellationToken)
        {
            var estate = mapper.Map<Estate>(request);
            var result = await repository.AddAsync(estate);
            if (result.IsSuccess)
            {
                return Result<Guid>.Success(result.Data);
            }
            return Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}
