using Application.Use_Cases.Queries.EstateQ;
using System.Text;

namespace Application.Utils.FilterStrategy
{
    public class BedroomsFilterStrategy : IFilterStrategy
    {
        public void ApplyFilter(StringBuilder sqlQuery, GetEstatesPaginationByFilterQuery request)
        {
            if (request.Bedrooms > 0)
            {
                sqlQuery.Append(" AND \"Bedrooms\" = ").Append(request.Bedrooms);
            }
        }
    }
}
