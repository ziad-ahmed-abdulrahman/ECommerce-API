namespace ECommerce.Api.Helper
{
    public class Pagination<T> where T : class
    {
        public Pagination(int pageNumber, int pageSize, int totalCount, IEnumerable<T>? data)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            Data = data;
        }

        public int PageNumber { set; get; }
        public int PageSize { set; get; }
        public int TotalCount { set; get; }
        public IEnumerable<T>?  Data{ set; get; }
    }
}
