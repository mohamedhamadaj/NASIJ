using TMAProject.Common;
using TMAProject.ViewModels.Admin.ColorVM;

namespace TMAProject.Services.Interfaces
{
    public interface IColorService
    {
        public Task<IEnumerable<ColorListVM>> GetAllAsync(CancellationToken cancellationToken = default);
        public Task<ColorEditVM?> GetOneAsync(Guid colorId, CancellationToken cancellationToken = default);
        public Task<ServiceResult> DeleteAsync(Guid colorId, CancellationToken cancellationToken = default);
        public Task<ServiceResult> UpdateAsync(ColorEditVM model, CancellationToken cancellationToken = default);
        public Task<ServiceResult> CreateAsync(ColorCreateVM model, CancellationToken cancellationToken = default);
    }
}
