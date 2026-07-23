using TMAProject.Models.Entities;
using TMAProject.ViewModels.Admin.ProductVM;

namespace TMAProject.Repository.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        public Task<IEnumerable<ProductListVM>> GetAllProductsAsync(CancellationToken cancellationToken = default);

        public Task<Product?> GetProductForEditAsync(Guid ProductId , CancellationToken cancellationToken = default);

        public Task<bool> IsNameExistAsync (string Name, Guid? ProductId,Guid categoryId, CancellationToken cancellationToken);

        public Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
        public void RemoveProductColor(ProductColor productColor);
        public void RemoveProductColorImage(ProductColorImage image);
        public void RemoveVariants(IEnumerable<ProductVariant> variants);

    }
}
