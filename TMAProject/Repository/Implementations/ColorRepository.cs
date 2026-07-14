using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TMAProject.DataAccess;
using TMAProject.Models.Entities;
using TMAProject.Repository.Interfaces;

namespace TMAProject.Repository.Implementations
{
    
    public class ColorRepository : GenericRepository<size> , IColorRepository
    {

        private readonly ApplicationDbContext _context;

        public ColorRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public  async Task<IEnumerable<size>> GetAllColorsAsync(CancellationToken cancellationToken)
        {
            return await _context.Colors
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsNameExistAsync(string name, Guid? colorId)
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
