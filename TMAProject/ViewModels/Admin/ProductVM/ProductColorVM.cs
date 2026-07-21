using TMAProject.Models.Entities;

namespace TMAProject.ViewModels.Admin.ProductVM
{
    public class ProductColorVM
    {
        public Guid? ProductColorId { get; set; }
        public Guid ColorId { get; set; }
        
        public List<ProductColorImageVM> ExistingImages { get; set; } = new List<ProductColorImageVM>();
        public List<IFormFile> NewImages { get; set; } = new List<IFormFile>();
        public List<ProductVariantVM> Variants { get; set; } = new List<ProductVariantVM>();

    }
}
