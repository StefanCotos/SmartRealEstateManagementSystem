namespace Application.Utils
{
    public class PagedResult<T>
    {
        public PagedResult(List<T> data, int totalCount)
        {
            Data = data;
            TotalCount = totalCount;
        }

        public List<T> Data { get; }
        public int TotalCount { get; }
    }
}
