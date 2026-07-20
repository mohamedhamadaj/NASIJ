using TMAProject.Common;
using TMAProject.ViewModels.Admin.SizeVM;

namespace TMAProject.Services.Interfaces
{
    public interface ISizeService
    {
        public Task<IEnumerable<SizeListVM>> GetAllSizeAsync(CancellationToken cancellationToken);

        public Task<SizeEditVM> GetOneAsync(Guid id, CancellationToken cancellationToken);

        public Task<ServiceResult> CreateAsync(SizeCreateVM model, CancellationToken cancellationToken);
        public Task<ServiceResult> UpdateAsync(SizeEditVM model, CancellationToken cancellationToken);
        public Task<ServiceResult> DeletAsync(Guid id, CancellationToken cancellationToken);
    }
}
