using Application.Use_Cases.Queries.EstateQ;
using System.Text;

namespace Application.Utils.FilterStrategy
{
    public interface IFilterStrategy
    {
        void ApplyFilter(StringBuilder sqlQuery, GetEstatesPaginationByFilterQuery request);
    }
}
