

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TMAProject.DataAccess;
using TMAProject.Models.Entities;
using TMAProject.Repository.Interfaces;

namespace TMAProject.Repository.Implementations
{
    public class SizeRepository : GenericRepository<Size> , ISizeRepository
    {
        private readonly ApplicationDbContext _context;

        public SizeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Size>> GetAllSizesAsync(CancellationToken cancellationToken)
        {
            return await _context.Sizes
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsNameExistAsync(string name, Guid? sizeId)
        {
            return await _context.Sizes
                .AnyAsync(c =>
                    c.Name == name &&
                    (!sizeId.HasValue || c.Id != sizeId.Value));
        }

        public async Task<IEnumerable<SelectListItem>> GetSizesForDropdownAsync(
            CancellationToken cancellationToken)
        {
            return await _context.Sizes
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
