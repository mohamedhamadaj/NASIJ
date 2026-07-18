using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TMAProject.Models.Entities;

namespace TMAProject.Repository.Interfaces
{
    public interface IColorRepository : IGenericRepository<Color>
        {
            public Task<IEnumerable<Color>> GetAllColorsAsync(CancellationToken cancellationToken);

        public Task<bool> IsNameExistAsync(string name, Guid? colorId);

        public Task<IEnumerable<SelectListItem>> GetColorsForDropdownAsync(CancellationToken cancellationToken);
        }

       
    
}
