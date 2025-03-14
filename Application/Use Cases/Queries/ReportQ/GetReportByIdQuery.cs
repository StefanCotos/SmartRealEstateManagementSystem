using Application.DTO;
using MediatR;

namespace Application.Use_Cases.Queries.Report
{
    public class GetReportByIdQuery : IRequest<ReportDto>
    {
        public Guid Id { get; set; }
    }
}
