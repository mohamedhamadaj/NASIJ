using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using TMAProject.Comomn;
using TMAProject.Repository.Interfaces;
using TMAProject.Services.Interfaces;
using TMAProject.ViewModels.Admin.ProductVM;

namespace TMAProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = Role.Admin)]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IColorRepository _colorRepository;
        private readonly ISizeRepository _sizeRepository;

        public ProductController(IProductService productService, ICategoryRepository categoryRepository, IColorRepository colorRepository, ISizeRepository sizeRepository)
        {
            _productService = productService;
            _categoryRepository = categoryRepository;
            _colorRepository = colorRepository;
            _sizeRepository = sizeRepository;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var product = await _productService.GetAllProductsAsync(cancellationToken);
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var model = new ProductCreatePageVM
            {
                Categories = await _categoryRepository.GetCategoriesForDropdownAsync(cancellationToken),
                Colors = await _colorRepository.GetColorsForDropdownAsync(cancellationToken),
                Sizes = await _sizeRepository.GetSizesForDropdownAsync(cancellationToken),
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreatePageVM model, CancellationToken cancellationToken)
        {
            Console.WriteLine($"===> [Create POST] Entered with Product Name: {model.Product?.ProductName}");
            Console.WriteLine($"===> [Create POST] Colors Count: {model.Product?.productColors?.Count}");
            Console.WriteLine($"===> [Create POST] ModelState.IsValid: {ModelState.IsValid}");

            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"===> [Create POST ModelError] Key: '{state.Key}' | Error: '{error.ErrorMessage}' | Exp: '{error.Exception?.Message}'");
                    }
                }

                model.Categories = await _categoryRepository.GetCategoriesForDropdownAsync(cancellationToken);
                model.Colors = await _colorRepository.GetColorsForDropdownAsync(cancellationToken);
                model.Sizes = await _sizeRepository.GetSizesForDropdownAsync(cancellationToken);
                return View(model);
            }

            var result = await _productService.CreateAsync(model.Product, cancellationToken);
            Console.WriteLine($"===> [Create POST] CreateAsync Result: Success={result.Success}, Message='{result.Message}'");

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                model.Categories = await _categoryRepository.GetCategoriesForDropdownAsync(cancellationToken);
                model.Colors = await _colorRepository.GetColorsForDropdownAsync(cancellationToken);
                model.Sizes = await _sizeRepository.GetSizesForDropdownAsync(cancellationToken);
                return View(model);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            var model = await _productService.GetOneAsync(id, cancellationToken);
            if (model == null)
            {
                return NotFound();
            }
            model.Categories = await _categoryRepository.GetCategoriesForDropdownAsync(cancellationToken);
            model.Colors = await _colorRepository.GetColorsForDropdownAsync(cancellationToken);
            model.Sizes = await _sizeRepository.GetSizesForDropdownAsync(cancellationToken);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductEditVM model, CancellationToken cancellationToken)
        {
            Console.WriteLine($"===> [Edit POST] Entered for ProductId: {model.ProductId}, Name: '{model.Name}'");
            Console.WriteLine($"===> [Edit POST] Colors Count: {model.ProductColors?.Count}");
            Console.WriteLine($"===> [Edit POST] ModelState.IsValid: {ModelState.IsValid}");

            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"===> [Edit POST ModelError] Key: '{state.Key}' | Error: '{error.ErrorMessage}' | Exp: '{error.Exception?.Message}'");
                    }
                }

                model.Categories = await _categoryRepository.GetCategoriesForDropdownAsync(cancellationToken);
                model.Colors = await _colorRepository.GetColorsForDropdownAsync(cancellationToken);
                model.Sizes = await _sizeRepository.GetSizesForDropdownAsync(cancellationToken);

                return View(model);
            }

            var result = await _productService.UpdateAsync(model, cancellationToken);
            Console.WriteLine($"===> [Edit POST] UpdateAsync Result: Success={result.Success}, Message='{result.Message}'");

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                model.Categories = await _categoryRepository.GetCategoriesForDropdownAsync(cancellationToken);
                model.Colors = await _colorRepository.GetColorsForDropdownAsync(cancellationToken);
                model.Sizes = await _sizeRepository.GetSizesForDropdownAsync(cancellationToken);
                return View(model);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _productService.DeleteAsync(id, cancellationToken);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = result.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}
