using Microsoft.EntityFrameworkCore;
using System.Drawing;
using TMAProject.Common;
using TMAProject.DataAccess;
using TMAProject.Models.Entities;
using TMAProject.Repository.Interfaces;
using TMAProject.Services.Interfaces;
using TMAProject.ViewModels.Admin.ProductVM;

namespace TMAProject.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IImageService _imageService;
        private readonly ApplicationDbContext _context;

        public ProductService(IProductRepository productRepository, IImageService imageService, ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _imageService = imageService;
            _context = context;
        }

        async Task<ServiceResult> IProductService.CreateAsync(ProductCreateVM model, CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine($"===> [CreateAsync] Started for product name: '{model.ProductName}'");
                var isExist = await _productRepository.IsNameExistAsync(model.ProductName, Guid.Empty, model.CategoryId, cancellationToken);

                if (isExist)
                {
                    Console.WriteLine($"===> [CreateAsync Failed] Product name '{model.ProductName}' already exists");
                    return ServiceResult.Fail("Product name already exists");
                }

                string? mainImageUrl = null;
                if (model.MainImage is null || model.MainImage.Length == 0)
                {
                    Console.WriteLine($"===> [CreateAsync Failed] Main image is missing or empty");
                    return ServiceResult.Fail("Main image is required.");
                }

                Console.WriteLine($"===> [CreateAsync] Uploading main image '{model.MainImage.FileName}' ({model.MainImage.Length} bytes)...");
                mainImageUrl = await _imageService.UploadImageAsync(model.MainImage, "Products", cancellationToken);
                Console.WriteLine($"===> [CreateAsync] Main image uploaded: '{mainImageUrl}'");

                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = model.ProductName,
                    Description = model.ProductDescription,
                    Price = model.ProductPrice,
                    MainImageUrl = mainImageUrl,
                    CategoryId = model.CategoryId,
                    Status = model.Status,
                };

                if (model.productColors != null && model.productColors.Any())
                {
                    Console.WriteLine($"===> [CreateAsync] Processing {model.productColors.Count} color groups...");

                    foreach (var color in model.productColors)
                    {
                        var productColor = new ProductColor
                        {
                            Id = Guid.NewGuid(),
                            ProductId = product.Id,
                            ColorId = color.ColorId
                        };

                        if (color.NewImages != null && color.NewImages.Any())
                        {
                            Console.WriteLine($"===> [CreateAsync] ColorId {color.ColorId} has {color.NewImages.Count} new images");
                            foreach (var image in color.NewImages.Where(i => i != null && i.Length > 0))
                            {
                                var imageUrl = await _imageService.UploadImageAsync(image, "Products/Colors", cancellationToken);
                                Console.WriteLine($"===> [CreateAsync] Uploaded color image: '{imageUrl}'");
                                productColor.Images.Add(new ProductColorImage
                                {
                                    Id = Guid.NewGuid(),
                                    ProductColorId = productColor.Id,
                                    ImageUrl = imageUrl
                                });
                            }
                        }

                        if (color.Variants != null && color.Variants.Any())
                        {
                            Console.WriteLine($"===> [CreateAsync] ColorId {color.ColorId} has {color.Variants.Count} variants");
                            foreach (var variant in color.Variants)
                            {
                                productColor.Variants.Add(new ProductVariant
                                {
                                    Id = Guid.NewGuid(),
                                    ProductColorId = productColor.Id,
                                    SizeId = variant.SizeId,
                                    Quantity = variant.Quantity,
                                    IsActive = variant.IsActive,
                                });
                            }
                        }

                        product.ProductColors.Add(productColor);
                    }
                }

                Console.WriteLine($"===> [CreateAsync] Saving product entity to DB...");
                await _productRepository.AddAsync(product, cancellationToken);
                await _productRepository.CommitAsync(cancellationToken);
                Console.WriteLine($"===> [CreateAsync] Product created successfully! Id={product.Id}");
                return ServiceResult.Ok("Product Created Successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"===> [CreateAsync EXCEPTION] {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
                return ServiceResult.Fail($"Error creating product: {ex.Message}");
            }
        }

        async Task<ServiceResult> IProductService.DeleteAsync(Guid ProductId, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProductForEditAsync(ProductId, cancellationToken);

            if (product == null)
                return ServiceResult.Fail("Product Not Exist");

            if (!string.IsNullOrEmpty(product.MainImageUrl))
            {
                await _imageService.DeleteImageAsync(product.MainImageUrl, "Products", cancellationToken);
            }

            if (product.ProductSubImages != null && product.ProductSubImages.Any())
            {
                foreach (var image in product.ProductSubImages)
                {
                    await _imageService.DeleteImageAsync(image.ImageUrl, "Products", cancellationToken);
                }
            }

            if (product.ProductColors != null && product.ProductColors.Any())
            {
                foreach (var color in product.ProductColors)
                {
                    if (color.Images != null)
                    {
                        foreach (var image in color.Images)
                        {
                            await _imageService.DeleteImageAsync(image.ImageUrl, "Products/Colors", cancellationToken);
                        }
                    }
                }
            }

            _productRepository.Delete(product);
            await _productRepository.CommitAsync(cancellationToken);
            return ServiceResult.Ok("Product Deleted Successfully");
        }

        async Task<IEnumerable<ProductListVM>> IProductService.GetAllProductsAsync(CancellationToken cancellationToken)
        {
            return await _productRepository.GetAllProductsAsync(cancellationToken);
        }

        async Task<ProductEditVM?> IProductService.GetOneAsync(Guid ProductId, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProductForEditAsync(ProductId, cancellationToken);

            if (product is null)
                return null;

            return new ProductEditVM
            {
                ProductId = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                DiscountPercentage = product.DiscountPercentage,
                CategoryId = product.CategoryId,
                ExistingMainImageUrl = product.MainImageUrl,
                Status = product.Status,

                ProductColors = product.ProductColors
                .Select(pc => new ProductColorVM
                {
                    ProductColorId = pc.Id,
                    ColorId = pc.ColorId,

                    ExistingImages = pc.Images
                    .Select(img => new ProductColorImageVM
                    {
                        ImageUrlId = img.Id,
                        ImageUrl = img.ImageUrl
                    }).ToList(),

                    Variants = pc.Variants
                    .Select(v => new ProductVariantVM
                    {
                        VariantId = v.Id,
                        SizeId = v.SizeId,
                        Quantity = v.Quantity,
                        IsActive = v.IsActive
                    }).ToList()
                })
                .ToList()
            };
        }

        public async Task<ServiceResult> UpdateAsync(ProductEditVM model, CancellationToken cancellationToken)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                Console.WriteLine($"===> [UpdateAsync] Started for ProductId '{model.ProductId}', Name '{model.Name}'");
                Console.WriteLine($"===> [UpdateAsync] ProductColors submitted: {model.ProductColors?.Count ?? 0}");

                // ── 1. Validation ──────────────────────────────────────────────────────────
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == model.ProductId, cancellationToken);

                if (product is null)
                {
                    Console.WriteLine($"===> [UpdateAsync Failed] Product with ID '{model.ProductId}' not found");
                    return ServiceResult.Fail("Product Not Exist");
                }

                var isExist = await _productRepository.IsNameExistAsync(
                    model.Name,
                    model.ProductId,
                    model.CategoryId,
                    cancellationToken);

                if (isExist)
                {
                    Console.WriteLine($"===> [UpdateAsync Failed] Product name '{model.Name}' already exists");
                    return ServiceResult.Fail("Product Name Already Exists");
                }

                // ── 2. Identify existing color groups and handle file deletions ────────────
                var existingColorIds = await _context.ProductColors
                    .Where(c => c.ProductId == model.ProductId)
                    .Select(c => c.Id)
                    .ToListAsync(cancellationToken);

                if (existingColorIds.Any())
                {
                    // Collect all existing image URLs in DB for disk cleanup
                    var dbColorImages = await _context.ProductColorImages
                        .Where(i => existingColorIds.Contains(i.ProductColorId))
                        .Select(i => i.ImageUrl)
                        .ToListAsync(cancellationToken);

                    // Collect all image URLs kept in form
                    var keptImageUrls = model.ProductColors != null
                        ? model.ProductColors
                            .Where(c => c.ExistingImages != null)
                            .SelectMany(c => c.ExistingImages)
                            .Select(img => img.ImageUrl)
                            .ToHashSet(StringComparer.OrdinalIgnoreCase)
                        : new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                    // Delete files from disk for removed images
                    foreach (var imgUrl in dbColorImages)
                    {
                        if (!string.IsNullOrEmpty(imgUrl) && !keptImageUrls.Contains(imgUrl))
                        {
                            await _imageService.DeleteImageAsync(imgUrl, "Products/Colors", cancellationToken);
                        }
                    }

                    // Execute SQL deletions directly
                    await _context.ProductVariants
                        .Where(v => existingColorIds.Contains(v.ProductColorId))
                        .ExecuteDeleteAsync(cancellationToken);

                    await _context.ProductColorImages
                        .Where(i => existingColorIds.Contains(i.ProductColorId))
                        .ExecuteDeleteAsync(cancellationToken);

                    await _context.ProductColors
                        .Where(c => c.ProductId == model.ProductId)
                        .ExecuteDeleteAsync(cancellationToken);
                }

                // ── 3. Clear ChangeTracker ────────────────────────────────────────────────
                _context.ChangeTracker.Clear();
                Console.WriteLine($"===> [UpdateAsync] ChangeTracker cleared successfully");

                // ── 4. Re-fetch clean Product entity ──────────────────────────────────────
                product = (await _context.Products.FirstOrDefaultAsync(p => p.Id == model.ProductId, cancellationToken))!;

                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;
                product.Status = model.Status;
                product.DiscountPercentage = model.DiscountPercentage;
                product.UpdatedAt = DateTime.UtcNow;

                // Update main image only if a valid new file was uploaded
                if (model.MainImage != null && model.MainImage.Length > 0)
                {
                    Console.WriteLine($"===> [UpdateAsync] Uploading new main image '{model.MainImage.FileName}'");
                    var oldMainImageUrl = product.MainImageUrl;
                    product.MainImageUrl = await _imageService.UploadImageAsync(model.MainImage, "Products", cancellationToken);

                    if (!string.IsNullOrEmpty(oldMainImageUrl))
                    {
                        await _imageService.DeleteImageAsync(oldMainImageUrl, "Products", cancellationToken);
                    }
                }

                // ── 5. Re-create color groups, color images, and variants ─────────────────
                if (model.ProductColors != null && model.ProductColors.Any())
                {
                    foreach (var colorVm in model.ProductColors)
                    {
                        var newColor = new ProductColor
                        {
                            Id = Guid.NewGuid(),
                            ProductId = product.Id,
                            ColorId = colorVm.ColorId,
                            Images = new List<ProductColorImage>(),
                            Variants = new List<ProductVariant>()
                        };

                        // Re-add kept existing color images
                        if (colorVm.ExistingImages != null)
                        {
                            foreach (var existingImg in colorVm.ExistingImages)
                            {
                                if (!string.IsNullOrEmpty(existingImg.ImageUrl))
                                {
                                    newColor.Images.Add(new ProductColorImage
                                    {
                                        Id = Guid.NewGuid(),
                                        ProductColorId = newColor.Id,
                                        ImageUrl = existingImg.ImageUrl
                                    });
                                }
                            }
                        }

                        // Add newly uploaded color images
                        if (colorVm.NewImages != null)
                        {
                            foreach (var image in colorVm.NewImages.Where(i => i != null && i.Length > 0))
                            {
                                var url = await _imageService.UploadImageAsync(image, "Products/Colors", cancellationToken);
                                Console.WriteLine($"===> [UpdateAsync] Uploaded new color image: '{url}'");
                                newColor.Images.Add(new ProductColorImage
                                {
                                    Id = Guid.NewGuid(),
                                    ProductColorId = newColor.Id,
                                    ImageUrl = url
                                });
                            }
                        }

                        // Add variants
                        if (colorVm.Variants != null)
                        {
                            foreach (var variantVm in colorVm.Variants)
                            {
                                newColor.Variants.Add(new ProductVariant
                                {
                                    Id = Guid.NewGuid(),
                                    ProductColorId = newColor.Id,
                                    SizeId = variantVm.SizeId,
                                    Quantity = variantVm.Quantity,
                                    IsActive = variantVm.IsActive
                                });
                            }
                        }

                        _context.ProductColors.Add(newColor);
                    }
                }

                Console.WriteLine($"===> [UpdateAsync] Committing updates to DB...");
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                Console.WriteLine($"===> [UpdateAsync SUCCESS] Product '{model.ProductId}' updated successfully!");
                return ServiceResult.Ok("Product Updated Successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                Console.WriteLine($"===> [UpdateAsync EXCEPTION] {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
                return ServiceResult.Fail($"Error updating product: {ex.Message}");
            }
        }
    }
}
