using BookWarehouse.Application.Comman.Results;

namespace BookWarehouse.Application.Abstractions
{
    public interface ICartService
    {

        Task<Result> AddToCart();
    }
}
