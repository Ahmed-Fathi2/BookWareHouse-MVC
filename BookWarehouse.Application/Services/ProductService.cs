using BookWarehouse.Application.Abstractions;
using BookWarehouse.Application.Comman;
using BookWarehouse.Application.Comman.Errors.Product;
using BookWarehouse.Application.ViewModels.Category;
using BookWarehouse.Application.ViewModels.Product;
using BookWarehouse.Domain.Entities;
using BookWarehouse.Domain.Repositories;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace BookWarehouse.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> CreateProduct(ProductCreateVM productCreateVM)
        {
            var product = productCreateVM.Adapt<Product>();

            _unitOfWork.ProductRepository.Add(product);

            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public Task<Result> DeleteProduct(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<IEnumerable<ProductReadVM>>> GetAllProducts()
        {
            var products = await _unitOfWork.ProductRepository.GetAllAsync(
                filter: x => !x.IsDeleted,
                includes: [ p => p.Category ]
                );

            var response = products.Adapt<IEnumerable<ProductReadVM>>();

            return Result.Success(response);
        }

        public async Task<Result<ProductReadDetailsVM>> GetProductById(Guid id)
        {
            var products = await _unitOfWork.ProductRepository.GetAllAsync(
                filter: p => p.Id == id && !p.IsDeleted,
                includes: [p => p.Category]);

            var product = products.FirstOrDefault();

            if (product == null)
                return Result.Failure<ProductReadDetailsVM>(ProductErrors.NotFound);

            var response = product.Adapt<ProductReadDetailsVM>();

            return Result.Success(response);



        }

        public Task<Result> UpdateProduct(ProductReadVM productReadVM)
        {
            throw new NotImplementedException();
        }
    }
}
