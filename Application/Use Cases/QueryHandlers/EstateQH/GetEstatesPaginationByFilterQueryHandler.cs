using Application.DTO;
using Application.Use_Cases.Queries.EstateQ;
using Application.Utils;
using Application.Utils.FilterStrategy;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Application.Use_Cases.QueryHandlers.EstateQH
{
    public class GetEstatesPaginationByFilterQueryHandler : IRequestHandler<GetEstatesPaginationByFilterQuery, Result<PagedResult<EstateDto>>>
    {
        private readonly IMapper mapper;
        private readonly ApplicationDbContext dbContext;
        private readonly List<IFilterStrategy> filterStrategies;

        public GetEstatesPaginationByFilterQueryHandler(IMapper mapper, ApplicationDbContext dbContext)
        {
            this.mapper = mapper;
            this.dbContext = dbContext;
            filterStrategies = new List<IFilterStrategy>
                    {
                        new NameFilterStrategy(),
                        new PriceFilterStrategy(),
                        new BedroomsFilterStrategy(),
                        new BathroomsFilterStrategy(),
                        new LandSizeFilterStrategy(),
                        new StreetFilterStrategy(),
                        new CityFilterStrategy(),
                        new StateFilterStrategy(),
                        new HouseSizeFilterStrategy()
                    };
        }

        public async Task<Result<PagedResult<EstateDto>>> Handle(GetEstatesPaginationByFilterQuery request, CancellationToken cancellationToken)
        {
            var sqlQuery = new StringBuilder("SELECT * FROM \"Estates\" WHERE \"IsSold\"=false");

            foreach (var strategy in filterStrategies)
            {
                strategy.ApplyFilter(sqlQuery, request);
            }

            var estates = await dbContext.Set<Estate>()
                .FromSqlRaw(sqlQuery.ToString())
                .ToListAsync(cancellationToken);

            var pagedEstates = estates.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();

            var estateDtos = mapper.Map<List<EstateDto>>(pagedEstates);

            var pagedResult = new PagedResult<EstateDto>(estateDtos, estates.Count);

            return Result<PagedResult<EstateDto>>.Success(pagedResult);
        }
    }
}