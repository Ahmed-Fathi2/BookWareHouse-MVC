using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman.Errors.Category;
using BookWarehouse.Application.Comman.Results;
using BookWarehouse.Application.ViewModels.Category;
using BookWarehouse.Domain.Entities;
using BookWarehouse.Domain.Repositories;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

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
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync(filter:x=>!x.IsDeleted);

            var response= categories.Adapt<IEnumerable<CategoryReadEditVM>>();

            return  Result.Success(response);
        }

        public async Task<Result<CategoryReadEditVM>> GetCategoryById(Guid id)
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

        public async Task<Result> DeleteCategory(Guid id)
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
