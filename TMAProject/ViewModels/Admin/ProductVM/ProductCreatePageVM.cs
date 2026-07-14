using Microsoft.AspNetCore.Mvc.Rendering;

namespace TMAProject.ViewModels.Admin.ProductVM
{
    public class ProductCreatePageVM
    {
        public ProductCreateVM Product { get; set; } = new();
        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Colors { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Sizes { get; set; } = new List<SelectListItem>();
    }
}
