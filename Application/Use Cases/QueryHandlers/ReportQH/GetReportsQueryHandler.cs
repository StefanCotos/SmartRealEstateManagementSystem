using Application.DTO;
using Application.Use_Cases.Queries.Report;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using MediatR;

namespace Application.Use_Cases.QueryHandlers.ReportQH
{
    public class GetReportsQueryHandler : IRequestHandler<GetReportsQuery, List<ReportDto>>
    {
        private readonly IGenericEntityRepository<Report> repository;
        private readonly IMapper mapper;

        public GetReportsQueryHandler(IGenericEntityRepository<Report> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<List<ReportDto>> Handle(GetReportsQuery request, CancellationToken cancellationToken)
        {
            var Reports = await repository.GetAllAsync();
            return mapper.Map<List<ReportDto>>(Reports);

        }
    }
}
