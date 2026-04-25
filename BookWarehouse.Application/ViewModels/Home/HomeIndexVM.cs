using BookWarehouse.Application.ViewModels.Category;
using BookWarehouse.Application.ViewModels.Product;

namespace BookWarehouse.Application.ViewModels.Home
{
    public class HomeIndexVM
    {
        public IEnumerable<CategoryReadEditVM> Categories { get; set; } = new List<CategoryReadEditVM>();
        public IEnumerable<ProductReadVM> FeaturedProducts { get; set; } = new List<ProductReadVM>();
    }
}
