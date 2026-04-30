using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman.Errors.Category;
using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.Category;
using BookWarehouse.Domain.Entities;
using BookWarehouse.Domain.Repositories;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookWarehouse.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

     
        public async Task<Result<IEnumerable<CategoryReadEditVM>>> GetAllCategories()
        {
            var query = await _unitOfWork.CategoryRepository.GetAllAsync(filter: x => !x.IsDeleted);

            var response = await query.ProjectToType<CategoryReadEditVM>().ToListAsync(); //Projection : select specific columns from db not all  
    

            return Result.Success((IEnumerable<CategoryReadEditVM>)response);
        }

  
        public async Task<Result<CategoryReadEditVM>> GetCategoryById(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category is null)
                return Result.Failure<CategoryReadEditVM>(CategoryErrors.CategoryNotFound);

            var response = category.Adapt<CategoryReadEditVM>();
           
            return Result.Success(response);

        }

        public async Task<Result> CreateCategory(CategoryCreateVM categoryCreateVM)
        {


            var category = categoryCreateVM.Adapt<Category>();
            _unitOfWork.CategoryRepository.Add(category); // Add to Local Containet so no need to await here
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> UpdateCategory(CategoryReadEditVM categoryReadEditVM)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryReadEditVM.Id);

            if (category is null)
                return Result.Failure<CategoryReadEditVM>(CategoryErrors.CategoryNotFound);


            categoryReadEditVM.Adapt(category);

            await _unitOfWork.SaveChangesAsync();

            return Result.Success();

        }

        public async Task<Result> DeleteCategory(int id)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);

            if (category is null)
                return Result.Failure<CategoryReadEditVM>(CategoryErrors.CategoryNotFound);


            _unitOfWork.CategoryRepository.Delete(category);

            await _unitOfWork.SaveChangesAsync();

            return Result.Success(); 

        }
    }
}
