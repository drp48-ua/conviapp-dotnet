using Microsoft.AspNetCore.Mvc;
using ConviAppWeb.Models;
using ConviAppWeb.DataAccess;

namespace ConviAppWeb.Controllers
{
    public class AccountController : Controller
    {
        // ─── LOGIN ───────────────────────────────────────────────
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
            var cad = new CADUsuario();
            var user = cad.BuscarPorEmail(email);

            if (user != null && user.PasswordHash == password)
            {
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Rol?.Nombre ?? "Basico");
                HttpContext.Session.SetString("UserName", user.Nombre ?? user.Email);
                HttpContext.Session.SetInt32("UserId", user.Id);
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Error = "Credenciales incorrectas. Prueba con dani@conviapp.com / 1234";
            return View();
        }

        // ─── REGISTER ────────────────────────────────────────────
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

            var cad = new CADUsuario();

            if (cad.ExisteEmail(email))
            {
                ViewBag.Error = "Ya existe una cuenta con ese email";
                return View();
            }

            var validPlans = new[] { "Basico", "Profesional", "Enterprise" };
            var assignedRole = validPlans.Contains(plan) ? plan! : "Basico";

            var user = new ENUsuario
            {
                Nombre       = name,
                Email        = email,
                PasswordHash = password,
                FechaRegistro= DateTime.Now,
                Activo       = true,
                Rol          = new ENRol { Nombre = assignedRole }
            };

            cad.CrearUsuario(user);

            // Auto-login — volvemos a leer para obtener el ID asignado
            var created = cad.BuscarPorEmail(email);
            if (created != null)
            {
                HttpContext.Session.SetString("UserEmail", created.Email);
                HttpContext.Session.SetString("UserRole", created.Rol?.Nombre ?? "Basico");
                HttpContext.Session.SetString("UserName", created.Nombre ?? created.Email);
                HttpContext.Session.SetInt32("UserId", created.Id);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        // ─── UPGRADE ─────────────────────────────────────────────
        [HttpGet]
        public IActionResult Upgrade(string plan)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (email == null) return RedirectToAction("Register", new { plan });

            var user = new CADUsuario().BuscarPorEmail(email);
            if (user == null) return RedirectToAction("Login");

            ViewBag.Plan      = plan;
            ViewBag.UserName  = user.Nombre;
            ViewBag.UserEmail = user.Email;
            return View();
        }

        [HttpPost]
        public IActionResult Upgrade(string email, string name, string accountNumber, string plan)
        {
            var sessionEmail = HttpContext.Session.GetString("UserEmail");
            if (sessionEmail == null) return RedirectToAction("Login");

            var cad  = new CADUsuario();
            var user = cad.BuscarPorEmail(sessionEmail);
            if (user == null) return RedirectToAction("Login");

            if (user.Email != email || user.Nombre != name)
            {
                ViewBag.Error     = "Los datos no coinciden con tu cuenta actual.";
                ViewBag.Plan      = plan;
                ViewBag.UserName  = user.Nombre;
                ViewBag.UserEmail = user.Email;
                return View();
            }

            if (string.IsNullOrWhiteSpace(accountNumber) || accountNumber.Length < 10)
            {
                ViewBag.Error     = "Número de cuenta inválido. Pon una cuenta simulada (ej. ES1234...).";
                ViewBag.Plan      = plan;
                ViewBag.UserName  = user.Nombre;
                ViewBag.UserEmail = user.Email;
                return View();
            }

            var validPlans = new[] { "Basico", "Profesional", "Enterprise" };
            user.Rol = new ENRol { Nombre = validPlans.Contains(plan) ? plan : "Basico" };
            cad.ActualizarUsuario(user);

            HttpContext.Session.SetString("UserRole", user.Rol.Nombre);
            return RedirectToAction("Index", "Dashboard");
        }

        // ─── LOGOUT ──────────────────────────────────────────────
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
