using Microsoft.AspNetCore.Mvc;
using ConviAppWeb.Models;
using System.Linq;

namespace ConviAppWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserEmail") != null)
                return RedirectToAction("Index", "Dashboard");
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user != null)
            {
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Role ?? "Basico");
                HttpContext.Session.SetString("UserName", user.Name ?? user.Email);
                HttpContext.Session.SetInt32("UserId", user.Id);
                return RedirectToAction("Index", "Dashboard");
            }
            ViewBag.Error = "Credenciales incorrectas. Prueba con dani@conviapp.com / 1234";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("UserEmail") != null)
                return RedirectToAction("Index", "Dashboard");
            return View();
        }

        [HttpPost]
        public IActionResult Register(string name, string email, string password, string confirmPassword, string? plan)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Las contraseñas no coinciden";
                return View();
            }

            if (_context.Users.Any(u => u.Email == email))
            {
                ViewBag.Error = "Ya existe una cuenta con ese email";
                return View();
            }

            // Validar que el plan sea uno de los permitidos
            var validPlans = new[] { "Basico", "Profesional", "Enterprise" };
            var assignedRole = validPlans.Contains(plan) ? plan! : "Basico";

            var user = new User
            {
                Name = name,
                Email = email,
                Password = password,
                Role = assignedRole
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Auto-login
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetString("UserName", user.Name ?? user.Email);
            HttpContext.Session.SetInt32("UserId", user.Id);

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
