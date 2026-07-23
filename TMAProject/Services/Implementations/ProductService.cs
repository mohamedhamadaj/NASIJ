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
            try
            {
                Console.WriteLine($"===> [UpdateAsync] Started for ProductId '{model.ProductId}', Name '{model.Name}'");
                Console.WriteLine($"===> [UpdateAsync] ProductColors submitted: {model.ProductColors?.Count ?? 0}");

                // ── 1. Validation ──────────────────────────────────────────────────────────
                var product = await _productRepository.GetProductForEditAsync(model.ProductId, cancellationToken);

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

                // ── 2. Collect entities and color images to delete ──────────────────────
                var filesToDelete = new List<(string url, string folder)>();

                var existingColors = product.ProductColors.ToList();
                Console.WriteLine($"===> [UpdateAsync] Existing colors count in DB: {existingColors.Count}");

                var submittedProductColorIds = model.ProductColors != null
                    ? model.ProductColors
                        .Where(c => c.ProductColorId.HasValue && c.ProductColorId.Value != Guid.Empty)
                        .Select(c => c.ProductColorId!.Value).ToHashSet()
                    : new HashSet<Guid>();
                var submittedColorIds = model.ProductColors != null
                    ? model.ProductColors.Select(c => c.ColorId).ToHashSet()
                    : new HashSet<Guid>();

                var colorGroupsToDelete = new List<Guid>();
                var imageIdsToDelete = new List<Guid>();
                var variantIdsToDelete = new List<Guid>();

                foreach (var dbColor in existingColors)
                {
                    bool isKept = submittedProductColorIds.Contains(dbColor.Id) || submittedColorIds.Contains(dbColor.ColorId);

                    if (!isKept)
                    {
                        Console.WriteLine($"===> [UpdateAsync] Color group '{dbColor.Id}' marked for deletion");
                        if (dbColor.Images != null)
                        {
                            foreach (var img in dbColor.Images)
                                filesToDelete.Add((img.ImageUrl, "Products/Colors"));
                        }
                        colorGroupsToDelete.Add(dbColor.Id);
                    }
                    else
                    {
                        var matchingVm = model.ProductColors?.FirstOrDefault(c =>
                            (c.ProductColorId.HasValue && c.ProductColorId.Value != Guid.Empty && c.ProductColorId.Value == dbColor.Id)
                            || c.ColorId == dbColor.ColorId);

                        if (matchingVm != null)
                        {
                            var keptImageIds = matchingVm.ExistingImages != null
                                ? matchingVm.ExistingImages.Select(img => img.ImageUrlId).ToHashSet()
                                : new HashSet<Guid>();

                            if (dbColor.Images != null)
                            {
                                foreach (var img in dbColor.Images.Where(i => !keptImageIds.Contains(i.Id)))
                                {
                                    filesToDelete.Add((img.ImageUrl, "Products/Colors"));
                                    imageIdsToDelete.Add(img.Id);
                                }
                            }

                            var keptVariantIds = matchingVm.Variants != null
                                ? matchingVm.Variants
                                    .Where(v => v.VariantId.HasValue && v.VariantId.Value != Guid.Empty)
                                    .Select(v => v.VariantId!.Value).ToHashSet()
                                : new HashSet<Guid>();
                            var keptSizeIds = matchingVm.Variants != null
                                ? matchingVm.Variants.Select(v => v.SizeId).ToHashSet()
                                : new HashSet<Guid>();

                            if (dbColor.Variants != null)
                            {
                                foreach (var v in dbColor.Variants.Where(v => !keptVariantIds.Contains(v.Id) && !keptSizeIds.Contains(v.SizeId)))
                                    variantIdsToDelete.Add(v.Id);
                            }
                        }
                    }
                }

                Console.WriteLine($"===> [UpdateAsync Deletions] Variants: {variantIdsToDelete.Count}, Images: {imageIdsToDelete.Count}, Colors: {colorGroupsToDelete.Count}, Files: {filesToDelete.Count}");

                // ── 3. Execute Deletions ────────────────────────────────────────────────
                foreach (var (url, folder) in filesToDelete)
                    await _imageService.DeleteImageAsync(url, folder, cancellationToken);

                if (variantIdsToDelete.Any())
                {
                    await _context.ProductVariants
                        .Where(v => variantIdsToDelete.Contains(v.Id))
                        .ExecuteDeleteAsync(cancellationToken);
                }

                if (imageIdsToDelete.Any())
                {
                    await _context.ProductColorImages
                        .Where(i => imageIdsToDelete.Contains(i.Id))
                        .ExecuteDeleteAsync(cancellationToken);
                }

                if (colorGroupsToDelete.Any())
                {
                    await _context.ProductColors
                        .Where(c => colorGroupsToDelete.Contains(c.Id))
                        .ExecuteDeleteAsync(cancellationToken);
                }

                // Clear Tracker after raw EF delete execution
                _context.ChangeTracker.Clear();
                Console.WriteLine($"===> [UpdateAsync] ChangeTracker cleared successfully");

                // ── 4. Re-fetch clean Product entity ────────────────────────────────────
                product = (await _productRepository.GetProductForEditAsync(model.ProductId, cancellationToken))!;

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
                    product.MainImageUrl =
                        await _imageService.UploadImageAsync(model.MainImage, "Products", cancellationToken);

                    if (!string.IsNullOrEmpty(oldMainImageUrl))
                    {
                        await _imageService.DeleteImageAsync(oldMainImageUrl, "Products", cancellationToken);
                    }
                }

                existingColors = product.ProductColors.ToList();

                if (model.ProductColors != null)
                {
                    foreach (var colorVm in model.ProductColors)
                    {
                        var dbColor = existingColors.FirstOrDefault(c =>
                            (colorVm.ProductColorId.HasValue && colorVm.ProductColorId.Value != Guid.Empty && c.Id == colorVm.ProductColorId.Value)
                            || c.ColorId == colorVm.ColorId);

                        if (dbColor == null)
                        {
                            Console.WriteLine($"===> [UpdateAsync] Adding NEW color group for ColorId '{colorVm.ColorId}'");
                            var newColor = new ProductColor
                            {
                                Id = Guid.NewGuid(),
                                ProductId = product.Id,
                                ColorId = colorVm.ColorId,
                                Images = new List<ProductColorImage>(),
                                Variants = new List<ProductVariant>()
                            };

                            if (colorVm.NewImages != null)
                            {
                                foreach (var image in colorVm.NewImages.Where(i => i != null && i.Length > 0))
                                {
                                    var url = await _imageService.UploadImageAsync(image, "Products/Colors", cancellationToken);
                                    Console.WriteLine($"===> [UpdateAsync] Uploaded new color image: '{url}'");
                                    newColor.Images.Add(new ProductColorImage { Id = Guid.NewGuid(), ProductColorId = newColor.Id, ImageUrl = url });
                                }
                            }

                            if (colorVm.Variants != null)
                            {
                                foreach (var variant in colorVm.Variants)
                                {
                                    newColor.Variants.Add(new ProductVariant
                                    {
                                        Id = Guid.NewGuid(),
                                        SizeId = variant.SizeId,
                                        Quantity = variant.Quantity,
                                        IsActive = variant.IsActive
                                    });
                                }
                            }

                            product.ProductColors.Add(newColor);
                        }
                        else
                        {
                            Console.WriteLine($"===> [UpdateAsync] Updating EXISTING color group ProductColorId '{dbColor.Id}'");
                            dbColor.ColorId = colorVm.ColorId;

                            if (colorVm.NewImages != null)
                            {
                                foreach (var image in colorVm.NewImages.Where(i => i != null && i.Length > 0))
                                {
                                    var url = await _imageService.UploadImageAsync(image, "Products/Colors", cancellationToken);
                                    Console.WriteLine($"===> [UpdateAsync] Uploaded additional color image for existing color: '{url}'");
                                    dbColor.Images.Add(new ProductColorImage
                                    {
                                        Id = Guid.NewGuid(),
                                        ProductColorId = dbColor.Id,
                                        ImageUrl = url
                                    });
                                }
                            }

                            var existingVariants = dbColor.Variants != null ? dbColor.Variants.ToList() : new List<ProductVariant>();

                            if (colorVm.Variants != null)
                            {
                                foreach (var variantVm in colorVm.Variants)
                                {
                                    ProductVariant? dbVariant = null;

                                    if (variantVm.VariantId.HasValue && variantVm.VariantId.Value != Guid.Empty)
                                        dbVariant = existingVariants.FirstOrDefault(v => v.Id == variantVm.VariantId.Value);

                                    if (dbVariant == null)
                                        dbVariant = existingVariants.FirstOrDefault(v => v.SizeId == variantVm.SizeId);

                                    if (dbVariant != null)
                                    {
                                        dbVariant.SizeId = variantVm.SizeId;
                                        dbVariant.Quantity = variantVm.Quantity;
                                        dbVariant.IsActive = variantVm.IsActive;
                                    }
                                    else
                                    {
                                        dbColor.Variants.Add(new ProductVariant
                                        {
                                            Id = Guid.NewGuid(),
                                            ProductColorId = dbColor.Id,
                                            SizeId = variantVm.SizeId,
                                            Quantity = variantVm.Quantity,
                                            IsActive = variantVm.IsActive
                                        });
                                    }
                                }
                            }
                        }
                    }
                }

                Console.WriteLine($"===> [UpdateAsync] Committing updates to DB...");
                await _productRepository.CommitAsync(cancellationToken);
                Console.WriteLine($"===> [UpdateAsync SUCCESS] Product '{model.ProductId}' updated successfully!");
                return ServiceResult.Ok("Product Updated Successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"===> [UpdateAsync EXCEPTION] {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
                return ServiceResult.Fail($"Error updating product: {ex.Message}");
            }
        }
    }
}
