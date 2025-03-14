using Application.Use_Cases.Queries.EstateQ;
using System.Text;

namespace Application.Utils.FilterStrategy
{
    public class LandSizeFilterStrategy : IFilterStrategy
    {
        public void ApplyFilter(StringBuilder sqlQuery, GetEstatesPaginationByFilterQuery request)
        {
            if (request.LandSize > 0)
            {
                sqlQuery.Append(" AND \"LandSize\" = ").Append(request.LandSize);
            }
        }
    }
}
