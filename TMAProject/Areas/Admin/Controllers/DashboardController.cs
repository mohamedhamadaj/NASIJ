using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMAProject.Comomn;

namespace TMAProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = Role.Admin)]

    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Products()
        {
            return View();
        }
    }
}
