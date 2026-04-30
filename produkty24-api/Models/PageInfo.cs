namespace Produkty24_API.Models
{
    public class PageInfo<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }

        public PageInfo(int totalPages, int currentPage, IEnumerable<T> items)
        {
            Items = items;
            CurrentPage = currentPage;
            TotalPages = totalPages;
        }

        public static int PagesCount(int totalCount, int pageSize)
        {
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            return Math.Max(1, totalPages);
        }
    }
}
