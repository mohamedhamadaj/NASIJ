using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using TMAProject.Models.Entities;
using TMAProject.Models.Enums;

namespace TMAProject.ViewModels.Admin.ProductVM
{
    public class ProductCreateVM
    {
        [Required]
        public string ProductName { get; set; } = string.Empty;
        public string? ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public Guid CategoryId { get; set; }
        public IFormFile? MainImage { get; set; }
        public List<IFormFile>? SubImages { get; set; } = [];
        public ProductStatus Status { get; set; }
        public List<ProductVariantVM> Variants { get; set; }= new List<ProductVariantVM>();



    }
}
