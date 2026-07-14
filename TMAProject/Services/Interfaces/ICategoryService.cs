using TMAProject.Common;
using TMAProject.ViewModels.Admin.CategoryVM;

namespace TMAProject.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryListVm>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<CategoryEditVM?> GetOneAsync (Guid categoryId, CancellationToken cancellationToken = default);

        Task<ServiceResult> CreateAsync(CategoryCreateVM model, CancellationToken cancellationToken = default);

        Task<ServiceResult> UpdateAsync(CategoryEditVM model, CancellationToken cancellationToken = default);
        Task<ServiceResult> DeleteAsync(Guid categoryId, CancellationToken cancellationToken = default);

    }
}
