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
        private readonly ApplicationDbContext _applicationDbContext;

        public ProductService(IProductRepository productRepository, IImageService imageService, ApplicationDbContext applicationDbContext)
        {
            _productRepository = productRepository;
            _imageService = imageService;
            _applicationDbContext = applicationDbContext;
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
            var product = await _productRepository.GetOneAsync(p => p.Id == ProductId, includes: [indexer => indexer.ProductSubImages, v => v.ProductColors.Select(pc=>pc.Images)]);

            if (product == null)
                return ServiceResult.Fail("Product Not Exist");


            if (!string.IsNullOrEmpty(product.MainImageUrl))
            {
                await _imageService.DeleteImageAsync(product.MainImageUrl, "Products", cancellationToken);
            }


            if (product.ProductSubImages.Any())
            {
                foreach (var image in product.ProductSubImages)
                {
                    await _imageService.DeleteImageAsync(image.ImageUrl, "Products", cancellationToken);
                }
                product.ProductSubImages.Clear();
            }

            foreach (var color in product.ProductColors)
            {
                foreach(var image in color.Images)
                {
                    await _imageService.DeleteImageAsync(image.ImageUrl, "Products/Colors", cancellationToken);
                }

            }

            _productRepository.Delete(product);
            await _productRepository.CommitAsync(cancellationToken);
            return ServiceResult.Ok("Product Deleted Succssefully");

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

                //ExistingSubImages = product.ProductSubImages
                //.Select(img => new ProductSubImageVM
                //{
                //    ImageId = img.Id,
                //    ImageUrl = img.ImageUrl
                //})
                //.ToList(),

                ProductColors = product.ProductColors
                .Select(pc => new ProductColorVM
                {
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

        async Task<ServiceResult> IProductService.UpdateAsync(ProductEditVM model, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProductForEditAsync(model.ProductId, cancellationToken);
            if (product is null)
            {
                return ServiceResult.Fail("Product Not Exist");
            }

            var isExist = await _productRepository.IsNameExistAsync(model.Name,model.ProductId, model.CategoryId, cancellationToken);

            if (isExist)
            {
                return ServiceResult.Fail("product name already exist");
            }

            product.Name = model.Name;
            product.CategoryId = model.CategoryId;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Status = model.Status;
            product.UpdatedAt = DateTime.UtcNow;
            product.DiscountPercentage = model.DiscountPercentage;

            if (model.MainImage != null)
            {
                if (!string.IsNullOrEmpty(product.MainImageUrl))
                {
                    await _imageService.DeleteImageAsync(product.MainImageUrl, "Products", cancellationToken);
                }

                product.MainImageUrl = await _imageService.UploadImageAsync(model.MainImage, "Products", cancellationToken);

            }
            // delete old sub image
            //if (product.ProductSubImages.Any())
            //{
            //    foreach (var image in product.ProductSubImages)
            //    {
            //        await _imageService.DeleteImageAsync(image.ImageUrl, "products", cancellationToken);
            //    }
            //    product.ProductSubImages.Clear();
            //}

            //if (model.NewSubImages is not null)
            //{
            //    foreach (var image in model.NewSubImages)
            //    {
            //        var imageurl = await _imageService.UploadImageAsync(image, "Products", cancellationToken);

            //        product.ProductSubImages.Add(new ProductSubImage
            //        {
            //            Id = Guid.NewGuid(),
            //            ImageUrl = imageurl,
            //            ProductId = product.Id,
            //        });
            //    }
            //}





            if (product.ProductColors.Any())
            {

                foreach(var color in product.ProductColors)
                {
                    foreach(var image in color.Images)
                    {
                        await _imageService.DeleteImageAsync(image.ImageUrl, "Products/Colors", cancellationToken);
                    }
                }


                _applicationDbContext.RemoveRange(product.ProductColors);
            }

            foreach (var color in model.ProductColors)
            {
                var productColor = new ProductColor
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    ColorId = color.ColorId
                };

                if (color.NewImages != null)
                {
                    foreach (var image in color.NewImages)
                    {
                        var imageUrl = await _imageService.UploadImageAsync(
                           image,
                           "Products/Colors",
                           cancellationToken);

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

            _productRepository.Update(product);
            await _productRepository.CommitAsync(cancellationToken);
            return ServiceResult.Ok("Product Updated Successfully");
        }
    }
}
