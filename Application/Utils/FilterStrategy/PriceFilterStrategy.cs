using Application.Use_Cases.Queries.EstateQ;
using System.Text;

namespace Application.Utils.FilterStrategy
{
    public class PriceFilterStrategy : IFilterStrategy
    {
        public void ApplyFilter(StringBuilder sqlQuery, GetEstatesPaginationByFilterQuery request)
        {
            if (request.Price > 0)
            {
                sqlQuery.Append(" AND \"Price\" = ").Append(request.Price);
            }
        }
    }
}