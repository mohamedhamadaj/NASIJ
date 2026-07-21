using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TMAProject.Comomn;
using TMAProject.Models.Entities;
using TMAProject.ViewModels.AccountVM;

namespace TMAProject.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var existingEmail = await _userManager.FindByEmailAsync(model.Email);

            if (existingEmail != null)
            {
                ModelState.AddModelError("", "Email already Exist");
                return View(model);
            }


            var existingUsername = await _userManager.FindByNameAsync(model.UserName);

            if (existingUsername != null)
            {
                ModelState.AddModelError("", "Username already Exist");
                return View(model);
            }


            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email,
                EmailConfirmed = true,
                IsActive = true
            };


            var result = await _userManager.CreateAsync(user, model.Passwod);


            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Role.Customer);

                await _signInManager.SignInAsync(user, isPersistent: false);


                return RedirectToAction(
                    nameof(Login)
                );
            }


            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }


            return View(model);
        }



        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var user =
                await _userManager.FindByEmailAsync(model.UserNameOrEmail)
                ??
                await _userManager.FindByNameAsync(model.UserNameOrEmail);



            if (user is null)
            {
                ModelState.AddModelError("", "Invalid User Name / Email Or Password");
                return View(model);
            }



            if (!user.IsActive)
            {
                ModelState.AddModelError("", "Your account has been disabled");
                return View(model);
            }



            var result = await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true
            );



            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(
                        "",
                        "Too many attempts, try again after 5 min"
                    );
                }
                else
                {
                    ModelState.AddModelError(
                        "",
                        "Invalid User Name / Email Or Password"
                    );
                }


                return View(model);
            }



            if (await _userManager.IsInRoleAsync(user, Role.Admin))
            {
                return RedirectToAction(
                    "Index",
                    "Dashboard",
                    new { area = "Admin" }
                );
            }



            return RedirectToAction(
                "Index",
                "Home"
            );
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();


            return RedirectToAction(
                "Index",
                "Home"
            );
        }
    }
}