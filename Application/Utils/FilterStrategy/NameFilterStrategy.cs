using Application.Use_Cases.Queries.EstateQ;
using System.Text;

namespace Application.Utils.FilterStrategy
{
    public class NameFilterStrategy : IFilterStrategy
    {
        public void ApplyFilter(StringBuilder sqlQuery, GetEstatesPaginationByFilterQuery request)
        {
            if (!string.IsNullOrEmpty(request.Name))
            {
                sqlQuery.Append(" AND LOWER(\"Name\") LIKE LOWER('%").Append(request.Name).Append("%')");
            }
        }
    }
}