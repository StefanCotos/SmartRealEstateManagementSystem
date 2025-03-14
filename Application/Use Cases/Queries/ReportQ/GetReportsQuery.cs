using Application.DTO;
using MediatR;

namespace Application.Use_Cases.Queries.Report
{
    public class GetReportsQuery:IRequest<List<ReportDto>>
    {
    }
}
