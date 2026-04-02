using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.Category;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Application.Abstractions
{
    public interface ICategoryService
    {

        Task<Result<IEnumerable<CategoryReadEditVM>>> GetAllCategories();
        Task<Result<CategoryReadEditVM>> GetCategoryById(Guid id);
        Task<Result>  CreateCategory(CategoryCreateVM categoryCreateVM);

        Task<Result> UpdateCategory( CategoryReadEditVM categoryReadEditVM);

        Task<Result> DeleteCategory(Guid id);

    }
}
