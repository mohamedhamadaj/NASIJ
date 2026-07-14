using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TMAProject.DataAccess;
using TMAProject.Models.Entities;
using TMAProject.Models.Enums;
using TMAProject.Repository.Interfaces;
using TMAProject.ViewModels.Admin.CategoryVM;

namespace TMAProject.Repository.Implementations
{
    public class CategoryRepository : GenericRepository<Category> ,ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context) 
        {
            _context = context;
        }

        public async Task<Category?> GetCategoryWithProductsAsync(Guid categoryId)
        {
            return await _context.Categories
                .AsNoTracking()
                .Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == categoryId);
        }

        public async Task<IEnumerable<CategoryListVm>> GetCategoriesWithProductCountAsync(CancellationToken cancellationToken)
        {
            return await _context.Categories.Select(c => new CategoryListVm
            {
                CategoryId = c.Id,
                CategoryName = c.Name,
                CategoryDescription = c.Description,
                CategoryImageUrl = c.ImageUrl,
                ProductCount = c.Products.Count(),
                Status = c.Status
            }).ToListAsync();
        }

        public async Task<bool> IsNameExistAsync(string name, Guid? id = null)
        {
            return await _context.Categories.AnyAsync(c => c.Name == name && (!id.HasValue || c.Id != id.Value));
        }

        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .Where(c => c.Status == CategoryStatus.Active).ToListAsync();
        }

        public async Task<bool> HasProductsAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .AnyAsync(p => p.CategoryId == categoryId, cancellationToken);
        }

        public async Task<IEnumerable<SelectListItem>> GetCategoriesForDropdownAsync(
            CancellationToken cancellationToken)
        {
            return await _context.Categories
                .AsNoTracking()
                .Where(c => c.Status == CategoryStatus.Active)
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
