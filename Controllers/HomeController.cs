using Microsoft.AspNetCore.Mvc;
using ConviAppWeb.Models;
using ConviAppWeb.Services;
using System.Linq;

namespace ConviAppWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public HomeController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            var featuredProperties = _context.Properties.Take(6).ToList();
            return View(featuredProperties);
        }

        public IActionResult Pisos()
        {
            var properties = _context.Properties.ToList();
            return View(properties);
        }

        public IActionResult PisoDetail(int id)
        {
            var property = _context.Properties.FirstOrDefault(p => p.Id == id);
            if (property == null) return NotFound();
            return View(property);
        }

        // ═══ LEGAL PAGES ═══
        public IActionResult Privacidad() => View();
        public IActionResult Terminos() => View();
        public IActionResult Cookies() => View();

        // ═══ CONTACT / ENTERPRISE APPLICATION ═══
        public IActionResult Contacto() => View();

        [HttpPost]
        public IActionResult Contacto(string name, string email, string company, string message)
        {
            var sent = _emailService.SendEnterpriseApplication(name, email, company, message);
            ViewBag.Success = sent;
            ViewBag.Message = sent
                ? "✅ Tu solicitud ha sido enviada. Nos pondremos en contacto contigo pronto."
                : "✅ Solicitud registrada. Te contactaremos en breve.";
            return View();
        }
    }
}
