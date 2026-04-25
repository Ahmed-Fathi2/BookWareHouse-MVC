using BookWarehouse.Application.Comman.Pagination;
using BookWarehouse.Application.ViewModels.Category;

namespace BookWarehouse.Application.ViewModels.Product
{
    public class CustomerProductIndexVM
    {
        public ProductQueryVM Query { get; set; } = new ProductQueryVM();
        public IEnumerable<CategoryReadEditVM> Categories { get; set; } = new List<CategoryReadEditVM>();
        public PagedResult<ProductReadVM> Products { get; set; } = default!;
    }
}
