using Application.Use_Cases.Queries.EstateQ;
using System.Text;

namespace Application.Utils.FilterStrategy
{
    public class CityFilterStrategy : IFilterStrategy
    {
        public void ApplyFilter(StringBuilder sqlQuery, GetEstatesPaginationByFilterQuery request)
        {
            if (!string.IsNullOrEmpty(request.City))
            {
                sqlQuery.Append(" AND LOWER(\"City\") LIKE LOWER('%").Append(request.City).Append("%')");
            }
        }
    }
}
