using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static int PagesCount(IQueryable<T> source, int pageSize)
        {
            var totalCount = source.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return Math.Max(1, totalPages);
        }
    }
}
