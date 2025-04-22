using Microsoft.AspNetCore.Mvc;
using QLQN.Data;
using QLQN.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace QLQN.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public IActionResult EditProfile()
        {
            var username = User.Identity.Name;
            var account = _context.Accounts
                .Include(a => a.User)
                .FirstOrDefault(a => a.Username == username);

            if (account?.User == null) return NotFound();

            var viewModel = new EditUserViewModel
            {
                Id = account.Id,
                FullName = account.User.FullName,
                Phone = account.User.Phone
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public IActionResult EditProfile(EditUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var account = _context.Accounts
                .Include(a => a.User)
                .FirstOrDefault(a => a.Id == model.Id);

            if (account == null || account.User == null) return NotFound();

            account.User.FullName = model.FullName;
            account.User.Phone = model.Phone;

            _context.SaveChanges();

            ViewBag.Message = "Cập nhật thành công!";
            return View(model);
        }


        public IActionResult Index()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username");
            return View();
        }
    }
}
