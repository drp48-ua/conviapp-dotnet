using Microsoft.AspNetCore.Mvc;
using ConviAppWeb.Models;
using ConviAppWeb.Services;
using ConviAppWeb.DataAccess;

namespace ConviAppWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly EmailService _emailService;

        public HomeController(EmailService emailService)
        {
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            var properties = new CADPiso().ListarTodos();
            return View(properties);
        }

        public IActionResult Pisos()
        {
            var properties = new CADPiso().ListarTodos();
            return View(properties);
        }

        public IActionResult PisoDetail(int id)
        {
            var piso = new CADPiso().LeerPiso(id);
            if (piso == null) return NotFound();
            return View(piso);
        }

        // ═══ LEGAL PAGES ═══
        public IActionResult Privacidad() => View();
        public IActionResult Terminos()   => View();
        public IActionResult Cookies()    => View();

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

        public IActionResult Error() => View();
    }
}
