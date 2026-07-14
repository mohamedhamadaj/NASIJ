namespace TMAProject.Models.Entities
{
    public class Size
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();

    }
}
