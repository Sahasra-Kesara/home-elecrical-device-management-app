using Microsoft.AspNetCore.Mvc;
using home_device_management_application.Models;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace home_device_management_application.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

                // Redirect based on role
                if (user.Role == "Admin")
                {
                    return RedirectToAction("AdminDashboard", "Dashboard");
                }
                else
                {
                    return RedirectToAction("UserDashboard", "Dashboard");
                }
            }

            ViewBag.Error = "Invalid login attempt.";
            return View();
        }


        [HttpPost]
        public IActionResult Register(string username, string password, string email, string role)
        {
            if (!_context.Users.Any(u => u.Username == username))
            {
                var newUser = new User
                {
                    Username = username,
                    Password = password,
                    Email = email,
                    Role = role // Admin/User
                };
                _context.Users.Add(newUser);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }

            ViewBag.Error = "Username already exists.";
            return View();
        }

        public async Task<IActionResult> LogoutAsync()
        {
            // Add logout logic here (e.g., clear session or cookies)
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login", "Account");
        }
    }
}
