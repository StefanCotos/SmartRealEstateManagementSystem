using Application.Use_Cases.Commands.ReviewUserC;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.ReviewUserCH
{
    public class CreateReviewUserCommandHandler : IRequestHandler<CreateReviewUserCommand, Result<Guid>>
    {
        private readonly IGenericEntityRepository<Review> repository;
        private readonly IMapper mapper;

        public CreateReviewUserCommandHandler(IGenericEntityRepository<Review> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<Result<Guid>> Handle(CreateReviewUserCommand request, CancellationToken cancellationToken)
        {
            var review = mapper.Map<Review>(request);
            var result = await repository.AddAsync(review);
            if (result.IsSuccess)
            {
                return Result<Guid>.Success(result.Data);
            }
            return Result<Guid>.Failure(result.ErrorMessage);
        }
    }
}
