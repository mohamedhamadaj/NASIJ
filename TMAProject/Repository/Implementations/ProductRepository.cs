using Microsoft.EntityFrameworkCore;
using TMAProject.DataAccess;
using TMAProject.Models.Entities;
using TMAProject.Models.Enums;
using TMAProject.Repository.Interfaces;
using TMAProject.ViewModels.Admin.ProductVM;

namespace TMAProject.Repository.Implementations
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {

        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        async Task<IEnumerable<Product>> IProductRepository.GetActiveProductsAsync(CancellationToken cancellationToken)
        {
            return await _context.Products
                .Where(p => p.Status == ProductStatus.Active)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        async Task<IEnumerable<ProductListVM>> IProductRepository.GetAllProductsAsync(CancellationToken cancellationToken)
        {
            return await _context.Products
                 .AsNoTracking()
                 .Select(p => new ProductListVM
                 {
                     ProductId = p.Id,
                     ProductName = p.Name,
                     Price = p.Price,
                     MainImage = p.MainImageUrl,
                     CategoryName = p.Category.Name,
                     DiscountPercentage = p.DiscountPercentage,
                     TotalStock = p.ProductColors
                     .SelectMany(pc=>pc.Variants).Sum(v => v.Quantity),
                     CreatedAt = p.CreatedAt,
                     Status = p.Status,
                 })
                 .ToListAsync(cancellationToken);

        }

        async Task<Product?> IProductRepository.GetProductForEditAsync(Guid ProductId, CancellationToken cancellationToken)
        {
            return await _context.Products
                 .Include(p => p.Category)
                 .Include(p => p.ProductSubImages)
                 .Include(p => p.ProductColors)
                 .ThenInclude(p => p.Color)
                 .Include(p => p.ProductColors)
                 .ThenInclude(p => p.Variants)
                 .ThenInclude(p=>p.Size)
                 .FirstOrDefaultAsync(p => p.Id == ProductId, cancellationToken);
        }

        async Task<bool> IProductRepository.IsNameExistAsync(string name, Guid? ProductId, Guid categoryId, CancellationToken cancellationToken)
        {
            return await _context.Products.AnyAsync(p => p.Name == name && p.CategoryId == categoryId && (!ProductId.HasValue || p.Id != ProductId.Value), cancellationToken);
        }
    }
}
