using ECommerce.Core.Entites.Product;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Core.DTOs
{

    public record ProductDto // for get 
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal NewPrice { get; set; }
        public decimal? OldPrice { get; set; }


        public virtual List<PhotoDto> Photos { get; set; } = new List<PhotoDto>();

        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }






    }
    public record ReturnProductDto // for get all endpoint
    {
        public List<Product>? Data { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

    }



    public record PhotoDto // for ProductDto
    {
        [MaxLength(100)]
        public string? ImageName { get; set; }

        public int ProductId { get; set; }

    }
    public record AddProductDto // for add
    {
        [MaxLength(50)]

        public string Name { get; set; } = null!;
        [MaxLength(200)]
        public string Description { get; set; } = null!;

        public decimal NewPrice { get; set; }
        public decimal? OldPrice { get; set; }

        public int CategoryId { get; set; }
        public IFormFileCollection? Photos { get; set; }

    }

    public record UpdateProductDto : AddProductDto  // for update 
    {

    }







}
