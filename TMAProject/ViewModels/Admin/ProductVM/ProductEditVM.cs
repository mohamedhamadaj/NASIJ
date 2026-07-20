using Microsoft.AspNetCore.Mvc.Rendering;
using TMAProject.Models.Entities;
using TMAProject.Models.Enums;
using TMAProject.ViewModels.Admin.ProductVM;

public class ProductEditVM
{
    public Guid ProductId { get; set; }

    public string Name { get; set; } = string.Empty;
    //public string? NewName { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public decimal? DiscountPercentage { get; set; }

    public Guid CategoryId { get; set; }

    public string? ExistingMainImageUrl { get; set; }

    public IFormFile? MainImage { get; set; }

    public List<ProductSubImageVM> ExistingSubImages { get; set; } = [];

    public List<IFormFile>? NewSubImages { get; set; }

    public ProductStatus Status { get; set; }

    public List<ProductColorVM>  ProductColors { get; set; } = [];
    public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Colors { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Sizes { get; set; } = new List<SelectListItem>();
}
