using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TMAProject.Models.Entities;
using TMAProject.ViewModels.Admin.ColorVM;

namespace TMAProject.Repository.Interfaces
{
    public interface IColorRepository : IGenericRepository<Color>
        {
            public Task<IEnumerable<ColorListVM>> GetAllColorsAsync(CancellationToken cancellationToken);

        public Task<bool> IsNameExistAsync(string name, Guid? colorId, CancellationToken cancellationToken = default);

        public Task<IEnumerable<SelectListItem>> GetColorsForDropdownAsync(CancellationToken cancellationToken);
        }

       
    
}
