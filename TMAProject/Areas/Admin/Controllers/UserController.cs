using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TMAProject.Comomn;
using TMAProject.Models.Entities;
using TMAProject.ViewModels.Admin.UserVM;

namespace TMAProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Role.Admin)]
    public class UserController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;


        public UserController(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }



        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();

            var model = new List<UserListVM>();


            foreach(var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);


                model.Add(new UserListVM
                {
                    Id = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    UserName = user.UserName!,
                    Email = user.Email!,
                    Role = roles.FirstOrDefault() ?? "",
                    IsActive = user.IsActive
                });
            }


            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);


            if(user is null)
            {
                return NotFound();
            }


            user.IsActive = !user.IsActive;


            await _userManager.UpdateAsync(user);


            TempData["Success"] = "User status updated";


            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
            {
                TempData["Error"] = "User not found";
                return RedirectToAction(nameof(Index));
            }


            // Prevent admin from deleting himself
            if (user.UserName == User.Identity!.Name)
            {
                TempData["Error"] = "You cannot delete your own account";
                return RedirectToAction(nameof(Index));
            }


            var result = await _userManager.DeleteAsync(user);


            if (!result.Succeeded)
            {
                TempData["Error"] = "Failed to delete user";
                return RedirectToAction(nameof(Index));
            }


            TempData["Success"] = "User deleted successfully";

            return RedirectToAction(nameof(Index));
        }

    }
}