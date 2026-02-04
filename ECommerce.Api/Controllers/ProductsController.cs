using AutoMapper;
using ECommerce.Api.Helper;
using ECommerce.Core.DTOs;
using ECommerce.Core.Entites.Product;
using ECommerce.Core.interfaces;
using ECommerce.Core.Services;
using ECommerce.Core.Sharing;
using ECommerce.Infrastructure.Repositories.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ECommerce.Api.Controllers
{
    [Route("api/products")]
    [ApiController]


    public class ProductsController : BaseController
    {
        private readonly IImageManagementService _imageManagementService;

        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper, IImageManagementService imageManagementService) : base(unitOfWork, mapper)
        {
            _imageManagementService = imageManagementService;
        }

        // get all products

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductParams productParams)
        {
            try
            {

                var products = await _unitOfWork.ProductRepositry.GetAllAsync(productParams);

                if (products.TotalCount == 0)
                    return Ok(new ResponseAPI<object>(200, "The store is currently empty.", new List<ProductDto>()));

                var result = _mapper.Map<List<ProductDto>>(products.Data);

                return Ok(new ResponseAPI<object>(200,
               products.TotalCount == 0
    ? "Welcome to our store! We're currently preparing our collection, please check back soon."
    : (productParams.PageNumber > products.TotalPages
        ? $"You've explored all our current products. We only have {products.TotalPages} pages available."
        : "Products retrieved successfully!"),
               new
               {
                   TotalCount = products.TotalCount,
                   PageSize = productParams.PageSize,
                   TotalPages = products.TotalPages,
                   PageNumber = productParams.PageNumber,
                   Data = result
               }));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<object>(400, message: ex.Message));
            }

        }

        //get product by id 

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var product = await _unitOfWork.ProductRepositry.GetByIdAsync(id, p => p.Category, p => p.Photos);
                if (product == null)
                    return BadRequest(new ResponseAPI<object>(400, "invalid id."));

                var result = _mapper.Map<ProductDto>(product);
                return Ok(new ResponseAPI<object>(200, "Data retrieved successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<object>(400, ex.Message));
            }

        }


        // add product
        // Admin
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromForm] AddProductDto addproductDto)
        {
            try
            {

                if (addproductDto.Photos != null)
                {

                    var product = _mapper.Map<Product>(source: addproductDto);
                    await _unitOfWork.ProductRepositry.AddAsync(product);

                    // proccesing name for photos
                    var cleanFileName = System.Text.RegularExpressions.Regex.Replace(addproductDto.Name.Trim(), @"[\s\n\r,]+", "-");

                    var ImagesPaths = await _imageManagementService.AddImageAsync(files: addproductDto.Photos, src: cleanFileName!);
                    var photos = ImagesPaths.Select(path => new Photo
                    {
                        ImageName = path,
                        ProductId = product.Id,

                    }).ToList();

                    bool result = await _unitOfWork.PhotoRepositry.AddPhotosAsync(photos);
                    if (!result)
                    {
                        return BadRequest(new ResponseAPI<object>(400, "Photos not saved."));
                    }

                    return Ok(new ResponseAPI<object>(200, "Product added successfully."));
                }
                else
                {
                    return BadRequest(new ResponseAPI<object>(400, "Please Add valid photos."));
                }

            }

            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<object>(400, ex.Message));
            }



        }


        // update product
        // Admin
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromForm] UpdateProductDto updateProductDto)
        {
            try
            {
                if (updateProductDto == null)
                    return BadRequest(new ResponseAPI<object>(400, "can not update empty product."));

                if (updateProductDto.Photos == null)
                    return BadRequest(new ResponseAPI<object>(400, "No photos provided."));



                var productFromDb = await _unitOfWork.ProductRepositry.GetByIdAsync(id,
                    p => p.Photos,
                    p => p.Category);

                if (productFromDb == null)
                    return BadRequest(new ResponseAPI<object>(400, "Invalid Id."));

                _mapper.Map(source: updateProductDto, productFromDb);

                var photosFromDb = productFromDb.Photos;

                // remove from storge 
                foreach (var photo in photosFromDb)
                {
                    await _imageManagementService.DeleteImageAsync(photo.ImageName);
                }

                //remove entities(ImageName,....) from Db
                await _unitOfWork.PhotoRepositry.RemovePhotosAsync(photosFromDb);

                // add photos in storge
                // proccesing name for photos
                var cleanFileName = System.Text.RegularExpressions.Regex.Replace(updateProductDto.Name.Trim(), @"[\s\n\r,]+", "-");

                var ImagesPaths = await _imageManagementService.AddImageAsync(files: updateProductDto.Photos, src: cleanFileName);
                var photos = ImagesPaths.Select(path => new Photo
                {
                    ImageName = path,
                    ProductId = id,

                }).ToList();

                // add photos to Db and update all chenges in Db such as Product and photos
                bool result = await _unitOfWork.PhotoRepositry.AddPhotosAsync(photos);

                // check db operation result
                if (!result)
                    return BadRequest(new ResponseAPI<object>(400, "failed to save photos to database."));

                return Ok(new ResponseAPI<object>(200, "product updated successfully"));

            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<object>(400, ex.Message));
            }

        }



        // Delete product
        // Admin
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _unitOfWork.ProductRepositry.DeleteProductWithPhotosAsync(id); // first check is found. second remove product and photos 
                if (result)
                {
                    return Ok(new ResponseAPI<object>(200, "item has been deleted successfully."));
                }
                else
                {
                    return BadRequest(new ResponseAPI<object>(400, "Invalid Id."));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<object>(400, ex.Message));
            }

        }
    }

}
