using TMAProject.Common;
using TMAProject.Models.Entities;
using TMAProject.Models.Enums;
using TMAProject.Repository.Interfaces;
using TMAProject.Services.Interfaces;
using TMAProject.ViewModels.Admin.CategoryVM;

namespace TMAProject.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IImageService _imageService;
        public CategoryService(ICategoryRepository categoryRepository, IImageService imageService)
        {
            _categoryRepository = categoryRepository;
            _imageService = imageService;
        }

        public async Task<ServiceResult>  CreateAsync(CategoryCreateVM model, CancellationToken cancellationToken)
        {
            var isexisting = await _categoryRepository.IsNameExistAsync(model.Name, Guid.Empty);

            if (isexisting)
            {
                return ServiceResult.Fail("Category name already exists.");
            }

            string? imageUrl = null;

            if (model.Image is not null)
            {
                imageUrl = await _imageService.UploadImageAsync(model.Image, "categories", cancellationToken);
            }
            var category = new Category
            {
                Name = model.Name,
                Description = model.Description,
                ImageUrl = imageUrl,
                Status = model.Status


            };
            await _categoryRepository.AddAsync(category, cancellationToken);
            await _categoryRepository.CommitAsync(cancellationToken);
            return ServiceResult.Ok("Category created successfully.");

        }

        public async Task<ServiceResult> DeleteAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetOneAsync(
                c => c.Id == categoryId,
                tracked: true,
                cancellationToken: cancellationToken);

            if (category is null)
            {
                return ServiceResult.Fail("Category not found.");
            }

            var hasProducts = await _categoryRepository.HasProductsAsync(
                categoryId,
                cancellationToken);

            if (hasProducts)
            {
                return ServiceResult.Fail("Cannot delete category because it contains products.");
            }

            if (!string.IsNullOrWhiteSpace(category.ImageUrl))
            {
                await _imageService.DeleteImageAsync(
                    category.ImageUrl,
                    "categories",
                    cancellationToken);
            }

            _categoryRepository.Delete(category);

            await _categoryRepository.CommitAsync(cancellationToken);

            return ServiceResult.Ok("Category deleted successfully.");
        }

        public async Task<IEnumerable<CategoryListVm>> GetAllAsync(CancellationToken cancellationToken = default)
        {
           return await _categoryRepository.GetCategoriesWithProductCountAsync(cancellationToken);
        }

        public async Task<CategoryEditVM?> GetOneAsync(Guid categoryId, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetOneAsync(c => c.Id == categoryId,
                tracked : false,
                cancellationToken: cancellationToken);
            if (category is null)
            {
                return null;
            }

            return  new CategoryEditVM
            {
                CategoryId = category.Id,
                Name = category.Name,
                Description = category.Description,
                ExistingImageUrl = category.ImageUrl,
                Status = category.Status
            };
            
        }

        public async Task<ServiceResult> UpdateAsync(CategoryEditVM model, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetOneAsync(c => c.Id == model.CategoryId, tracked: true, cancellationToken: cancellationToken);
                 
                if (category is null)
                {
                    return ServiceResult.Fail("Category not found.");
                }
                var isexisting = await _categoryRepository.IsNameExistAsync(model.Name, model.CategoryId);
                if (isexisting)
                {
                    return ServiceResult.Fail("Category name already exists.");
                }

                category.Name = model.Name;
                category.Description = model.Description;
           if (model.Image is not null)
{
    if (!string.IsNullOrWhiteSpace(category.ImageUrl))
    {
        await _imageService.DeleteImageAsync(
            category.ImageUrl,
            "categories",
            cancellationToken);
    }

    category.ImageUrl = await _imageService.UploadImageAsync(
        model.Image,
        "categories",
        cancellationToken);
}
                await _categoryRepository.CommitAsync(cancellationToken);
                return ServiceResult.Ok("Category updated successfully.");
            
        }
    }
}
