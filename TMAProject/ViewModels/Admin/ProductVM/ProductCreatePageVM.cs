using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TMAProject.ViewModels.Admin.ProductVM
{
    public class ProductCreatePageVM
    {
        public ProductCreateVM Product { get; set; } = new();
        [ValidateNever]
        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
        [ValidateNever]
        public IEnumerable<SelectListItem> Colors { get; set; } = new List<SelectListItem>();
        [ValidateNever]
        public IEnumerable<SelectListItem> Sizes { get; set; } = new List<SelectListItem>();
    }
}
