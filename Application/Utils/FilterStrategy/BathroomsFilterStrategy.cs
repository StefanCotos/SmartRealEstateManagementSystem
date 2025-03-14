using Application.Use_Cases.Queries.EstateQ;
using System.Text;

namespace Application.Utils.FilterStrategy
{
    public class BathroomsFilterStrategy : IFilterStrategy
    {
        public void ApplyFilter(StringBuilder sqlQuery, GetEstatesPaginationByFilterQuery request)
        {
            if (request.Bathrooms > 0)
            {
                sqlQuery.Append(" AND \"Bathrooms\" = ").Append(request.Bathrooms);
            }
        }
    }
}
