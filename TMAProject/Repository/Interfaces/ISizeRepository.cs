using Microsoft.AspNetCore.Mvc.Rendering;
using TMAProject.Models.Entities;

namespace TMAProject.Repository.Interfaces
{
    public interface ISizeRepository
    {
        public Task<IEnumerable<Size>> GetAllSizesAsync(CancellationToken cancellationToken);

        public Task<bool> IsNameExistAsync(string name, Guid? sizeId);

        public Task<IEnumerable<SelectListItem>> GetSizesForDropdownAsync(CancellationToken cancellationToken);
    }
}
