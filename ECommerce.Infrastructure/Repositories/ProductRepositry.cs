using ECommerce.Core.DTOs;
using ECommerce.Core.Entites.Product;
using ECommerce.Core.interfaces;
using ECommerce.Core.Services;
using ECommerce.Core.Sharing;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories
{
    internal class ProductRepositry : GenericRepositry<Product>, IProductRepositry
    {
        private readonly IImageManagementService _imageManagementService;
        public ProductRepositry(AppDbContext appDbContext ,IImageManagementService imageManagementService) : base(appDbContext)
        {
            _imageManagementService = imageManagementService;
        }

        public async Task<ReturnProductDto> GetAllAsync(ProductParams productParams) //default page 1, 3 elemnts Per Page, sort by name, optional filtering 
        {
            var query = _appDbContext.Products
                   .Include(p => p.Photos)
                   .Include(navigationPropertyPath: p => p.Category)
                   .AsNoTracking();

            
            // filtering by category
            if (productParams.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == productParams.CategoryId);

            //filtering by keyword
            if (!string.IsNullOrEmpty(value: productParams.Search))
            {
                var searchWords = productParams.Search.Split(' ');  
                query = query.Where(p => searchWords.All(word => 
                p.Name.ToLower().Contains( word.ToLower())
                || 
                p.Description.ToLower().Contains(word.ToLower())
                ));
            } 

            if (!string.IsNullOrEmpty(productParams.Sort)) 
            {
                query = productParams.Sort switch
                {
                    "PriceAsc" => query.OrderBy(p => p.NewPrice),
                    "PriceDesc" => query.OrderByDescending(p => p.NewPrice),
                    _ => query.OrderBy(p => p.Name),
                };
            }
            else
            {
                query =  query.OrderBy(p => p.Name);
            }

            // total count before pagination
            var totalCount =await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / productParams.PageSize);


            // pagination
            var products = await query.Skip(count: (productParams.PageSize) * (productParams.PageNumber - 1)).Take(count: productParams.PageSize)
                .ToListAsync();

            return new ReturnProductDto 
            { 
                Data = products,
                TotalCount = totalCount,   
                TotalPages = totalPages,
            }; 
        }
        public async Task<bool> DeleteProductWithPhotosAsync(int id)
        {
            var product =await GetByIdAsync(id, p => p.Photos);

            if (product == null)
               return false;

            foreach(Photo photo in product.Photos)
            {

               await _imageManagementService.DeleteImageAsync(photo.ImageName);
            }

            _appDbContext.RemoveRange(entities: product.Photos);
            _appDbContext.Remove(product);

            await _appDbContext.SaveChangesAsync();

            return true;


        }

        
    } 
}
