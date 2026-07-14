using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMAProject.Comomn;
using TMAProject.Models.Entities;
using TMAProject.Models.Enums;
using TMAProject.Repository.Interfaces;
using TMAProject.Services.Interfaces;
using TMAProject.ViewModels.Admin.CategoryVM;

namespace TMAProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = Role.Admin)]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IImageService _imageService;

        public CategoryController(ICategoryRepository categoryRepository, IImageService imageService)
        {
            _categoryRepository = categoryRepository;
            _imageService = imageService;
        }

        // GET: Admin/Category
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetCategoriesWithProductCountAsync(cancellationToken);
            return View(categories);
        }

        // GET: Admin/Category/Create
        public IActionResult Create()
        {
            return View(new CategoryCreateVM());
        }

        // POST: Admin/Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateVM model, CancellationToken cancellationToken)
        {
            if (await _categoryRepository.IsNameExistAsync(model.Name, null))
            {
                ModelState.AddModelError("Name", "Category name already exists.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? imageUrl = null;
            if (model.Image != null)
            {
                imageUrl = await _imageService.UploadImageAsync(model.Image, "categories", cancellationToken);
            }

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                ImageUrl = imageUrl,
                Status = model.Status
            };

            await _categoryRepository.AddAsync(category, cancellationToken);
            await _categoryRepository.CommitAsync(cancellationToken);

            TempData["Success"] = "Category created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Category/Edit/{id}
        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id, cancellationToken: cancellationToken);
            if (category == null)
            {
                return NotFound();
            }

            var model = new CategoryEditVM
            {
                CategoryId = category.Id,
                Name = category.Name,
                Description = category.Description,
                ExistingImageUrl = category.ImageUrl,
                Status = category.Status
            };

            return View(model);
        }

        // POST: Admin/Category/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryEditVM model, CancellationToken cancellationToken)
        {
            if (await _categoryRepository.IsNameExistAsync(model.Name, model.CategoryId))
            {
                ModelState.AddModelError("Name", "Category name already exists.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var category = await _categoryRepository.GetOneAsync(c => c.Id == model.CategoryId, cancellationToken: cancellationToken);
            if (category == null)
            {
                return NotFound();
            }

            category.Name = model.Name;
            category.Description = model.Description;
            category.Status = model.Status;

            if (model.Image != null)
            {
                if (!string.IsNullOrEmpty(category.ImageUrl))
                {
                    try
                    {
                        await _imageService.DeleteImageAsync(category.ImageUrl, "categories", cancellationToken);
                    }
                    catch
                    {
                        // Ignore image delete failure
                    }
                }
                category.ImageUrl = await _imageService.UploadImageAsync(model.Image, "categories", cancellationToken);
            }

            _categoryRepository.Update(category);
            await _categoryRepository.CommitAsync(cancellationToken);

            TempData["Success"] = "Category updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Category/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id, cancellationToken: cancellationToken);
            if (category == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(category.ImageUrl))
            {
                try
                {
                    await _imageService.DeleteImageAsync(category.ImageUrl, "categories", cancellationToken);
                }
                catch(Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            _categoryRepository.Delete(category);
            await _categoryRepository.CommitAsync(cancellationToken);

            TempData["Success"] = "Category deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
