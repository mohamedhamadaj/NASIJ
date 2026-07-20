namespace TMAProject.ViewModels.Admin.ProductVM
{
    public class ProductVariantVM
    {
        public Guid? VariantId { get; set; }
        public Guid SizeId { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
    }
}
