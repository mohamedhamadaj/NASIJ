using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TMAProject.Models.Entities;
using TMAProject.ViewModels.Admin.IdentityVM;

namespace TMAProject.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;


        public ProfileController(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }



        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);


            if (user is null)
            {
                return NotFound();
            }


            var model = new ProfileVM
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber
            };


            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var user = await _userManager.GetUserAsync(User);


            if (user is null)
            {
                return NotFound();
            }


            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Address = model.Address;
            user.PhoneNumber = model.PhoneNumber;


            var result = await _userManager.UpdateAsync(user);


            if (result.Succeeded)
            {
                TempData["Success"] = "Profile updated successfully";

                return RedirectToAction(nameof(Index));
            }


            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }


            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var user = await _userManager.GetUserAsync(User);


            if (user is null)
            {
                return NotFound();
            }



            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword
            );



            if (result.Succeeded)
            {
                // refresh cookie after password change
                await _userManager.UpdateSecurityStampAsync(user);


                TempData["Success"] = "Password changed successfully";


                return RedirectToAction(nameof(Index));
            }



            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }


            return View(model);
        }
    }
}
