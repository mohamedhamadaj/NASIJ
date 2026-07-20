using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TMAProject.DataAccess;
using TMAProject.Models.Entities;
using TMAProject.Repository.Interfaces;
using TMAProject.ViewModels.Admin.ColorVM;

namespace TMAProject.Repository.Implementations
{
    
    public class ColorRepository : GenericRepository<Color> , IColorRepository
    {

        private readonly ApplicationDbContext _context;

        public ColorRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public  async Task<IEnumerable<ColorListVM>> GetAllColorsAsync(CancellationToken cancellationToken)
        {
            return await _context.Colors
                .AsNoTracking()
                .Select(c => new ColorListVM
                {
                    ColorId = c.Id,
                    Name = c.Name,
                }).ToListAsync(cancellationToken);
        }

        public async Task<bool> IsNameExistAsync(string name, Guid? colorId, CancellationToken cancellationToken = default)
        {
            return await _context.Colors
                .AnyAsync(c =>
                    c.Name == name &&
                    (!colorId.HasValue || c.Id != colorId.Value));
        }

        public async Task<IEnumerable<SelectListItem>> GetColorsForDropdownAsync(
            CancellationToken cancellationToken)
        {
            return await _context.Colors
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync(cancellationToken);
        }
    }
}
