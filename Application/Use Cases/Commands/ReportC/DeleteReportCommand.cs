using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.ReportC
{
    public record DeleteReportCommand(Guid Id) : IRequest<Result<Guid>>;
}
