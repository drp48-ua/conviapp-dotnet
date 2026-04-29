using Microsoft.AspNetCore.Mvc;
using ConviAppWeb.Models;
using System.Linq;

namespace ConviAppWeb.Controllers
{
    public class AccountController : Controller
    {
        public AccountController()
        {
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserEmail") != null)
                return RedirectToAction("Index", "Dashboard");
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password, string? plan)
        {
            // Reemplazo a ADO.NET: Se asume validacion
            if (email != null && password != null)
            {
                var validPlans = new[] { "Basico", "Profesional", "Enterprise" };
                var role = validPlans.Contains(plan) ? plan! : "Basico";
                HttpContext.Session.SetString("UserEmail", email);
                HttpContext.Session.SetString("UserRole", role);
                HttpContext.Session.SetString("UserName", email);
                HttpContext.Session.SetInt32("UserId", 1);
                return RedirectToAction("Index", "Dashboard");
            }
            ViewBag.Error = "Credenciales incorrectas.";
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

            var validPlans = new[] { "Basico", "Profesional", "Enterprise" };
            var assignedRole = validPlans.Contains(plan) ? plan! : "Basico";

            var cadUser = new ConviAppWeb.DataAccess.CADUsuario();
            cadUser.CrearUsuario(new ConviAppWeb.Models.ENUsuario
            {
                Nombre = name ?? "Usuario",
                Email = email,
                PasswordHash = password,
                RolId = 1
            });

            // Auto-login
            HttpContext.Session.SetString("UserEmail", email);
            HttpContext.Session.SetString("UserRole", assignedRole);
            HttpContext.Session.SetString("UserName", name ?? email);
            HttpContext.Session.SetInt32("UserId", 1);

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult Upgrade(string plan)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (email == null)
            {
                // Usuario no logueado -> va al registro normal
                return RedirectToAction("Register", new { plan = plan });
            }

            ViewBag.Plan = plan;
            ViewBag.UserName = "Usuario";
            ViewBag.UserEmail = email;
            return View();
        }

        [HttpPost]
        public IActionResult Upgrade(string email, string name, string accountNumber, string plan)
        {
            var sessionEmail = HttpContext.Session.GetString("UserEmail");
            if (sessionEmail == null) return RedirectToAction("Login");

            if (string.IsNullOrWhiteSpace(accountNumber) || accountNumber.Length < 10)
            {
                ViewBag.Error = "Número de cuenta inválido. Ponga una cuenta simulada (ej. ES1234...).";
                ViewBag.Plan = plan;
                ViewBag.UserName = name;
                ViewBag.UserEmail = email;
                return View();
            }

            // Cambiar el plan del usuario mock logic
            var validPlans = new[] { "Basico", "Profesional", "Enterprise" };
            var role = validPlans.Contains(plan) ? plan : "Basico";

            // Refrescar sesión
            HttpContext.Session.SetString("UserRole", role);

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (email == null) return RedirectToAction("Login");
            
            ViewBag.UserName = HttpContext.Session.GetString("UserName") ?? email;
            ViewBag.UserEmail = email;
            ViewBag.UserPhoto = HttpContext.Session.GetString("UserPhotoUrl") ?? "";
            
            return View();
        }

        [HttpPost]
        public IActionResult Profile(string name, string email, string photoUrl)
        {
            var sessionEmail = HttpContext.Session.GetString("UserEmail");
            if (sessionEmail == null) return RedirectToAction("Login");

            // Mock update, ideally we'd use cadUser.UpdateUser
            HttpContext.Session.SetString("UserName", name ?? email);
            HttpContext.Session.SetString("UserEmail", email);
            if (!string.IsNullOrEmpty(photoUrl)) {
                HttpContext.Session.SetString("UserPhotoUrl", photoUrl);
            }
            
            ViewBag.Success = "Perfil actualizado correctamente.";
            ViewBag.UserName = name ?? email;
            ViewBag.UserEmail = email;
            ViewBag.UserPhoto = HttpContext.Session.GetString("UserPhotoUrl") ?? "";

            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
