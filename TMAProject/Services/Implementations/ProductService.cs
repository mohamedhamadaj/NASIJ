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
        //private readonly ApplicationDbContext _applicationDbContext;

        public ProductService(IProductRepository productRepository, IImageService imageService/*, ApplicationDbContext applicationDbContext*/)
        {
            _productRepository = productRepository;
            _imageService = imageService;
            //_applicationDbContext = applicationDbContext;
        }

        async Task<ServiceResult> IProductService.CreateAsync(ProductCreateVM model, CancellationToken cancellationToken)
        {
            var isExist = await _productRepository.IsNameExistAsync(model.ProductName, Guid.Empty, model.CategoryId, cancellationToken);

            if (isExist)
            {
                return ServiceResult.Fail("product name already exist");
            }

            string? mainImageUrl = null;
            if (model.MainImage is null)
            {
                return ServiceResult.Fail("Main image is required.");
            }

            if (model.MainImage is not null)
            {
                mainImageUrl = await _imageService.UploadImageAsync(model.MainImage, "Products", cancellationToken);
            }

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


            foreach(var color in model.productColors)
            {
                var productColor = new ProductColor
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    ColorId = color.ColorId
                };

                if(color.NewImages != null)
                {
                    foreach(var image in color.NewImages)
                    {
                        var imageUrl = await _imageService.UploadImageAsync(image,"Products/Colors",cancellationToken);
                        productColor.Images.Add(new ProductColorImage
                        {
                            Id = Guid.NewGuid(),
                            ImageUrl = imageUrl
                        });
                    }
                }

                foreach(var variant in color.Variants)
                {
                    productColor.Variants.Add(new ProductVariant
                    {
                        Id = Guid.NewGuid(),
                        SizeId = variant.SizeId,
                        Quantity = variant.Quantity,
                        IsActive = variant.IsActive,
                    });
                }

                product.ProductColors.Add(productColor);


            }


            // general sub images
            //if (model.SubImages is not null)
            //{
            //    foreach (var subImage in model.SubImages)
            //    {
            //        var imageUrl = await _imageService.UploadImageAsync(subImage, "Products", cancellationToken);

            //        product.ProductSubImages.Add(new ProductSubImage
            //        {
            //            Id = Guid.NewGuid(),
            //            ImageUrl = imageUrl,
            //        });
            //    }
            //}

            await _productRepository.AddAsync(product, cancellationToken);
            await _productRepository.CommitAsync(cancellationToken);
            return ServiceResult.Ok("Product Created Successfully");

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
            var product = await _productRepository.GetProductForEditAsync(model.ProductId, cancellationToken);

            if (product is null)
                return ServiceResult.Fail("Product Not Exist");

            var isExist = await _productRepository.IsNameExistAsync(
                model.Name,
                model.ProductId,
                model.CategoryId,
                cancellationToken);

            if (isExist)
                return ServiceResult.Fail("Product Name Already Exists");

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.CategoryId = model.CategoryId;
            product.Status = model.Status;
            product.DiscountPercentage = model.DiscountPercentage;
            product.UpdatedAt = DateTime.UtcNow;

            if (model.MainImage != null)
            {
                if (!string.IsNullOrEmpty(product.MainImageUrl))
                    await _imageService.DeleteImageAsync(
                        product.MainImageUrl,
                        "Products",
                        cancellationToken);

                product.MainImageUrl =
                    await _imageService.UploadImageAsync(
                        model.MainImage,
                        "Products",
                        cancellationToken);
            }

            var existingColors = product.ProductColors.ToList();
            var submittedProductColorIds = model.ProductColors != null
                ? model.ProductColors.Where(c => c.ProductColorId.HasValue && c.ProductColorId.Value != Guid.Empty).Select(c => c.ProductColorId!.Value).ToHashSet()
                : new HashSet<Guid>();
            var submittedColorIds = model.ProductColors != null
                ? model.ProductColors.Select(c => c.ColorId).ToHashSet()
                : new HashSet<Guid>();

            // 1. Delete color groups removed by user
            foreach (var dbColor in existingColors)
            {
                bool isKept = submittedProductColorIds.Contains(dbColor.Id) || submittedColorIds.Contains(dbColor.ColorId);

                if (!isKept)
                {
                    if (dbColor.Images != null)
                    {
                        foreach (var image in dbColor.Images)
                        {
                            await _imageService.DeleteImageAsync(image.ImageUrl, "Products/Colors", cancellationToken);
                        }
                    }
                    _productRepository.RemoveProductColor(dbColor);
                }
            }

            // 2. Add or Update submitted color groups
            if (model.ProductColors != null)
            {
                foreach (var colorVm in model.ProductColors)
                {
                    var dbColor = existingColors.FirstOrDefault(c => 
                        (colorVm.ProductColorId.HasValue && colorVm.ProductColorId.Value != Guid.Empty && c.Id == colorVm.ProductColorId.Value) 
                        || c.ColorId == colorVm.ColorId);

                    if (dbColor == null)
                    {
                        // Brand new color group
                        var newColor = new ProductColor
                        {
                            Id = Guid.NewGuid(),
                            ProductId = product.Id,
                            ColorId = colorVm.ColorId,
                            Images = new List<ProductColorImage>(),
                            Variants = new List<ProductVariant>()
                        };

                        if (colorVm.NewImages != null && colorVm.NewImages.Any())
                        {
                            foreach (var image in colorVm.NewImages)
                            {
                                var url = await _imageService.UploadImageAsync(image, "Products/Colors", cancellationToken);
                                newColor.Images.Add(new ProductColorImage
                                {
                                    Id = Guid.NewGuid(),
                                    ImageUrl = url
                                });
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
                        // Update existing color group
                        dbColor.ColorId = colorVm.ColorId;

                        // a. Clean up deleted images
                        var keptImageIds = colorVm.ExistingImages != null
                            ? colorVm.ExistingImages.Select(img => img.ImageUrlId).ToHashSet()
                            : new HashSet<Guid>();

                        if (dbColor.Images != null)
                        {
                            var deletedImages = dbColor.Images.Where(i => !keptImageIds.Contains(i.Id)).ToList();
                            foreach (var image in deletedImages)
                            {
                                await _imageService.DeleteImageAsync(image.ImageUrl, "Products/Colors", cancellationToken);
                                _productRepository.RemoveProductColorImage(image);
                            }
                        }

                        // b. Add new uploaded images
                        if (colorVm.NewImages != null && colorVm.NewImages.Any())
                        {
                            foreach (var image in colorVm.NewImages)
                            {
                                var url = await _imageService.UploadImageAsync(image, "Products/Colors", cancellationToken);
                                dbColor.Images.Add(new ProductColorImage
                                {
                                    Id = Guid.NewGuid(),
                                    ProductColorId = dbColor.Id,
                                    ImageUrl = url
                                });
                            }
                        }

                        // c. Update variants in-place safely
                        var existingVariants = dbColor.Variants != null ? dbColor.Variants.ToList() : new List<ProductVariant>();
                        var submittedVariantIds = colorVm.Variants != null
                            ? colorVm.Variants.Where(v => v.VariantId.HasValue && v.VariantId.Value != Guid.Empty).Select(v => v.VariantId!.Value).ToHashSet()
                            : new HashSet<Guid>();
                        var submittedSizeIds = colorVm.Variants != null
                            ? colorVm.Variants.Select(v => v.SizeId).ToHashSet()
                            : new HashSet<Guid>();

                        // Delete removed variants
                        var deletedVariants = existingVariants.Where(v => !submittedVariantIds.Contains(v.Id) && !submittedSizeIds.Contains(v.SizeId)).ToList();
                        if (deletedVariants.Any())
                        {
                            _productRepository.RemoveVariants(deletedVariants);
                        }

                        // Update or add variants
                        if (colorVm.Variants != null)
                        {
                            foreach (var variantVm in colorVm.Variants)
                            {
                                ProductVariant? dbVariant = null;

                                if (variantVm.VariantId.HasValue && variantVm.VariantId.Value != Guid.Empty)
                                {
                                    dbVariant = existingVariants.FirstOrDefault(v => v.Id == variantVm.VariantId.Value);
                                }
                                if (dbVariant == null)
                                {
                                    dbVariant = existingVariants.FirstOrDefault(v => v.SizeId == variantVm.SizeId);
                                }

                                if (dbVariant != null)
                                {
                                    // Update existing variant in place
                                    dbVariant.SizeId = variantVm.SizeId;
                                    dbVariant.Quantity = variantVm.Quantity;
                                    dbVariant.IsActive = variantVm.IsActive;
                                }
                                else
                                {
                                    // Add new variant
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

            await _productRepository.CommitAsync(cancellationToken);
            return ServiceResult.Ok("Product Updated Successfully");
        }
    }
}
