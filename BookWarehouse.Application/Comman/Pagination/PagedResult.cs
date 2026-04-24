using Microsoft.EntityFrameworkCore;

namespace BookWarehouse.Application.Comman.Pagination
{
    public class PagedResult<T> // Dto of pagination result
    {
        public PagedResult(List<T> items, int pageNumber, int pageSize, int totalCount)
        {
            Items = items;
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        }
        public List<T> Items { get; private set; } = new List<T>();
        public int PageNumber { get; private set; }

        public int TotalPages { get; private set; }

        public bool IsPrevious => PageNumber > 1;

        public bool IsNext => PageNumber < TotalPages;


        public static async Task<PagedResult<T>> Create(IQueryable<T> source, int pageNumber, int pageSize)
        {

            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<T>(items, pageNumber, pageSize, count);
        }
    }
}
