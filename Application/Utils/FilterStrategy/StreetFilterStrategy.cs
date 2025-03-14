using Application.Use_Cases.Queries.EstateQ;
using System.Text;

namespace Application.Utils.FilterStrategy
{
    public class StreetFilterStrategy : IFilterStrategy
    {
        public void ApplyFilter(StringBuilder sqlQuery, GetEstatesPaginationByFilterQuery request)
        {
            if (!string.IsNullOrEmpty(request.Street))
            {
                sqlQuery.Append(" AND LOWER(\"Street\") LIKE LOWER('%").Append(request.Street).Append("%')");
            }
        }
    }
}
