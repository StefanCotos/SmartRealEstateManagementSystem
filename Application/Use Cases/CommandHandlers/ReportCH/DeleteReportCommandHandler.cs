using Application.Use_Cases.Commands.ReportC;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.CommandHandlers.ReportCH
{
    public class DeleteReportCommandHandler : IRequestHandler<DeleteReportCommand, Result<Guid>>
    {
        private readonly IGenericEntityRepository<Report> repository;
        public DeleteReportCommandHandler(IGenericEntityRepository<Report> repository)
        {
            this.repository = repository;
        }
        public async Task<Result<Guid>> Handle(DeleteReportCommand request, CancellationToken cancellationToken)
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
