using TMAProject.Common;
using TMAProject.ViewModels.Admin.ProductVM;

namespace TMAProject.Services.Interfaces
{
    public interface IProductService
    {
        public Task<IEnumerable<ProductListVM>> GetAllProductsAsync(CancellationToken cancellationToken);

        public Task<ProductEditVM?> GetOneAsync(Guid ProductId, CancellationToken cancellationToken);

        public Task<ServiceResult> CreateAsync(ProductCreateVM model, CancellationToken cancellationToken);

        public Task<ServiceResult> UpdateAsync(ProductEditVM model, CancellationToken cancellationToken);

        public Task<ServiceResult> DeleteAsync(Guid ProductId, CancellationToken cancellationToken);
    }
}
