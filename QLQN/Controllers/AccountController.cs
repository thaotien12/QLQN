using Microsoft.AspNetCore.Mvc;
using QLQN.Data;
using QLQN.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace QLQN.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (_context.Accounts.Any(a => a.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                return View(model);
            }

            // Tạo tài khoản
            var account = new Account
            {
                Username = model.Username,
                PasswordHash = HashPassword(model.Password),
                Role = "User"
            };
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            // Tạo người dùng
            var user = new User
            {
                AccountId = account.Id,
                FullName = "",  // Hoặc model.FullName nếu có
                Phone = ""
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Tạo claims và đăng nhập luôn
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, account.Username),
        new Claim(ClaimTypes.Role, account.Role),
        new Claim("AccountId", account.Id.ToString()),
        new Claim("UserId", user.Id.ToString())
    };

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal);

            // Chuyển hướng cập nhật thông tin cá nhân
            return RedirectToAction("EditProfile", "User");
        }



        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Account model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.PasswordHash))
            {
                ModelState.AddModelError("", "Vui lòng nhập tên đăng nhập và mật khẩu.");
                return View();
            }

            // Mã hoá mật khẩu người dùng nhập vào
            var hashedPassword = ComputeSha256Hash(model.PasswordHash);

            // Truy vấn account
            var account = await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Username == model.Username && a.PasswordHash == hashedPassword);

            if (account == null)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View(model);
            }

            // Lưu session
            var claims = new List<Claim>
{
    new Claim(ClaimTypes.Name, account.Username),
    new Claim(ClaimTypes.Role, account.Role),
    new Claim("AccountId", account.Id.ToString())
};

            if (account.User != null)
                claims.Add(new Claim("UserId", account.User.Id.ToString()));

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal);


            // Chuyển hướng
            if (account.Role == "Admin")
                return RedirectToAction("Index", "Admin");
            else
                return RedirectToAction("Index", "User");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // Hàm mã hoá SHA256
        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
    }
}

   
