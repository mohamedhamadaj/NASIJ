using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMAProject.Comomn;
using TMAProject.Services.Interfaces;
using TMAProject.ViewModels.Admin.SizeVM;

namespace TMAProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = Role.Admin)]
    public class SizeController : Controller
    {
        private readonly ISizeService _sizeService;

        public SizeController(ISizeService sizeService)
        {
            _sizeService = sizeService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var sizes = await _sizeService.GetAllSizeAsync(cancellationToken);
            return View(sizes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SizeCreateVM model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _sizeService.CreateAsync(model, cancellationToken);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            var model = await _sizeService.GetOneAsync(id, cancellationToken);

            if (model is null)
                return NotFound();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SizeEditVM model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _sizeService.UpdateAsync(model, cancellationToken);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sizeService.DeletAsync(id, cancellationToken);

            TempData[result.Success ? "Success" : "Error"] = result.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}
