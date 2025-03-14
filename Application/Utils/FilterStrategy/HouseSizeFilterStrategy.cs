using Application.Use_Cases.Queries.EstateQ;
using System.Text;

namespace Application.Utils.FilterStrategy
{
    public class HouseSizeFilterStrategy : IFilterStrategy
    {
        public void ApplyFilter(StringBuilder sqlQuery, GetEstatesPaginationByFilterQuery request)
        {
            if (request.HouseSize > 0)
            {
                sqlQuery.Append(" AND \"HouseSize\" = ").Append(request.HouseSize);
            }
        }
    }
}
