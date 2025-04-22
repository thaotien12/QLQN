using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace QLQN.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            return View();
        }
    }
}
