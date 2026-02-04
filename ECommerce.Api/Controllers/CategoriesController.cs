using AutoMapper;
using ECommerce.Api.Helper;
using ECommerce.Core.DTOs;
using ECommerce.Core.Entites.Product;
using ECommerce.Core.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
    [Route("api/categories")]
    [ApiController]
    

    public class CategoriesController : BaseController
    {
        public CategoriesController(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() 
        {
            try
            {
                var categories = await _unitOfWork.CategoryRepositry.GetAllAsync();
                if(categories == null)
                    return BadRequest(new ResponseAPI<object>(404, "We couldn’t find any categories"));

                return Ok(new ResponseAPI<IEnumerable<Category>>(200, "Categories retrieved successfully", categories));
            }
            catch (Exception ex)
            {
               
                return BadRequest(new ResponseAPI<object>(400, ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) 
        {
            try
            {
                Category? category = await _unitOfWork.CategoryRepositry.GetByIdAsync(id);
                if (category == null)
                {
                    return NotFound(new ResponseAPI<object>(404, "The specified category was not found."));
                }

                return Ok(new ResponseAPI<Category>(200, "Category retrieved successfully", category));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<object>(400, ex.Message));
            }

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add( CategoryDto categoryDto) 
        {
            try
            {
                var category = _mapper.Map<Category>(categoryDto);

                 await _unitOfWork.CategoryRepositry.AddAsync(category);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = category.Id },
                    new ResponseAPI<object>(201, "Category has been added")
                );

            }

            catch (Exception ex)
            {
                
                return BadRequest(new ResponseAPI<object>(400, ex.Message));
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute]int id, [FromBody]UpdateCategoryDto updateDto)
        {
            
            var existingCategory = await _unitOfWork.CategoryRepositry.GetByIdAsync(id);

            if (existingCategory == null)
                return NotFound(new ResponseAPI<object>(404, "Category not found"));

            if (updateDto.Name != null)
                existingCategory.Name = updateDto.Name;

            if (updateDto.Description != null)
                existingCategory.Description = updateDto.Description;

           
            await _unitOfWork.CategoryRepositry.UpdateAsync(existingCategory);

            return Ok(new ResponseAPI<object>(200, "Category updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _unitOfWork.CategoryRepositry.DeleteAsync(id);
                return Ok(new ResponseAPI<object>(200, "Category deleted successfully"));


            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<object>(400, ex.Message));
            }

        }


    }


}
