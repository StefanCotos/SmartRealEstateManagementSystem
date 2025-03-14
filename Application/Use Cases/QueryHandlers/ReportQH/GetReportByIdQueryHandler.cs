using Application.DTO;
using Application.Use_Cases.Queries.Report;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.ReportQH
{
    public class GetReportByIdQueryHandler : IRequestHandler<GetReportByIdQuery, ReportDto>
    {
        private readonly IGenericEntityRepository<Report> repository;
        private readonly IMapper mapper;

        public GetReportByIdQueryHandler(IGenericEntityRepository<Report> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<ReportDto> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
        {
            var Report = await repository.GetByIdAsync(request.Id);
            return mapper.Map<ReportDto>(Report);

        }
    }
}
