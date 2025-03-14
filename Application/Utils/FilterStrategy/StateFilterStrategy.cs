using Application.Use_Cases.Queries.EstateQ;
using System.Text;

namespace Application.Utils.FilterStrategy
{
    public class StateFilterStrategy : IFilterStrategy
    {
        public void ApplyFilter(StringBuilder sqlQuery, GetEstatesPaginationByFilterQuery request)
        {
            if (!string.IsNullOrEmpty(request.State))
            {
                sqlQuery.Append(" AND LOWER(\"State\") LIKE LOWER('%").Append(request.State).Append("%')");
            }
        }
    }
}
