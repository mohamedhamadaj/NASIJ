using Microsoft.AspNetCore.Mvc.Rendering;
using TMAProject.Models.Entities;
using TMAProject.ViewModels.Admin.SizeVM;

namespace TMAProject.Repository.Interfaces
{
    public interface ISizeRepository : IGenericRepository<Size>
    {
        public Task<IEnumerable<SizeListVM>> GetAllSizesAsync(CancellationToken cancellationToken);

        public Task<bool> IsNameExistAsync(string name, Guid? sizeId, CancellationToken cancellationToken = default);

        public Task<IEnumerable<SelectListItem>> GetSizesForDropdownAsync(CancellationToken cancellationToken);
    }
}
