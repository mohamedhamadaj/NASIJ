

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TMAProject.DataAccess;
using TMAProject.Models.Entities;
using TMAProject.Repository.Interfaces;
using TMAProject.ViewModels.Admin.SizeVM;

namespace TMAProject.Repository.Implementations
{
    public class SizeRepository : GenericRepository<Size> , ISizeRepository
    {
        private readonly ApplicationDbContext _context;

        public SizeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<SizeListVM>> GetAllSizesAsync(CancellationToken cancellationToken)
        {
            return await _context.Sizes
                .AsNoTracking()
                .Select(s=> new SizeListVM
                {
                    SizeId = s.Id,
                    SizeName = s.Name,
                }).ToListAsync(cancellationToken);
        }

        public async Task<bool> IsNameExistAsync(string name, Guid? sizeId, CancellationToken cancellationToken = default)
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
