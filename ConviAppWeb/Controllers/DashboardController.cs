using Microsoft.AspNetCore.Mvc;
using ConviAppWeb.Models;
using ConviAppWeb.Services;
using ConviAppWeb.DataAccess;
using System.IO;

namespace ConviAppWeb.Controllers
{
    public class DashboardController : Controller
    {
        private ENUsuario? GetCurrentUser()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (email == null) return null;
            return new CADUsuario().BuscarPorEmail(email);
        }

        private IActionResult? RequireLogin()
        {
            if (GetCurrentUser() == null)
                return RedirectToAction("Login", "Account");
            return null;
        }

        private int GetUserPisoId() => 1; // Piso por defecto

        // ═══ INICIO / RESUMEN ════════════════════════════════════════
        public IActionResult Index()
        {
            var r = RequireLogin(); if (r != null) return r;
            var user = GetCurrentUser()!;
            ViewBag.UserRole = user.Rol?.Nombre ?? "Basico";
            ViewBag.UserName = user.Nombre ?? user.Email;

            var piso = new CADPiso().LeerPiso(GetUserPisoId());
            return View(piso);
        }

        // ═══ TAREAS ══════════════════════════════════════════════════
        public IActionResult Tareas()
        {
            var r = RequireLogin(); if (r != null) return r;
            var user = GetCurrentUser()!;
            ViewBag.UserRole = user.Rol?.Nombre ?? "Basico";
            var tasks = new CADTarea().ListarTodas(GetUserPisoId());
            return View(tasks);
        }

        [HttpPost]
        public IActionResult AddTask(string titulo, string descripcion, string prioridad)
        {
            var user = GetCurrentUser();
            if (user == null) return RedirectToAction("Login", "Account");
            new CADTarea().CrearTarea(new ENTarea
            {
                Titulo      = titulo,
                Descripcion = descripcion,
                Prioridad   = prioridad ?? "media",
                Estado      = "pendiente",
                CreadaPorId = user.Id,
                PisoId      = GetUserPisoId()
            });
            return RedirectToAction("Tareas");
        }

        [HttpPost]
        public IActionResult CompleteTask(int id)
        {
            new CADTarea().ToggleEstado(id);
            return RedirectToAction("Tareas");
        }

        [HttpPost]
        public IActionResult DeleteTask(int id)
        {
            new CADTarea().BorrarTarea(new ENTarea { Id = id });
            return RedirectToAction("Tareas");
        }

        // ═══ GASTOS ══════════════════════════════════════════════════
        public IActionResult Gastos()
        {
            var r = RequireLogin(); if (r != null) return r;
            var user = GetCurrentUser()!;
            ViewBag.UserRole = user.Rol?.Nombre ?? "Basico";
            var expenses = new CADGasto().ListarTodos(GetUserPisoId());
            return View(expenses);
        }

        [HttpPost]
        public IActionResult AddExpense(string concepto, decimal importe)
        {
            var user = GetCurrentUser();
            if (user == null) return RedirectToAction("Login", "Account");
            new CADGasto().CrearGasto(new ENGasto
            {
                Concepto        = concepto,
                Importe         = importe,
                Fecha           = DateTime.Now,
                RegistradoPorId = user.Id,
                PisoId          = GetUserPisoId()
            });
            return RedirectToAction("Gastos");
        }

        [HttpPost]
        public IActionResult DeleteExpense(int id)
        {
            new CADGasto().BorrarGasto(new ENGasto { Id = id });
            return RedirectToAction("Gastos");
        }

        // ═══ MENSAJES ════════════════════════════════════════════════
        public IActionResult Mensajes()
        {
            var r = RequireLogin(); if (r != null) return r;
            var user = GetCurrentUser()!;
            ViewBag.UserRole = user.Rol?.Nombre ?? "Basico";
            var messages = new CADMensaje().ListarTodos(GetUserPisoId());
            return View(messages);
        }

        [HttpPost]
        public IActionResult SendMessage(string text)
        {
            var user = GetCurrentUser();
            if (user == null || string.IsNullOrEmpty(text)) return RedirectToAction("Mensajes");
            new CADMensaje().CrearMensaje(new ENMensaje
            {
                Contenido  = text,
                FechaEnvio = DateTime.Now,
                EmisorId   = user.Id,
                PisoId     = GetUserPisoId()
            });
            return RedirectToAction("Mensajes");
        }

        // ═══ RESERVAS ════════════════════════════════════════════════
        public IActionResult Reservas()
        {
            var r = RequireLogin(); if (r != null) return r;
            var user = GetCurrentUser()!;
            ViewBag.UserRole = user.Rol?.Nombre ?? "Basico";
            if (!PlanService.CanAccessReservas(user.Rol?.Nombre ?? "Basico"))
            {
                ViewBag.Locked = true;
                return View(new List<ENReserva>());
            }
            var reservations = new CADReserva().ListarTodas(user.Id);
            return View(reservations);
        }

        [HttpPost]
        public IActionResult AddReservation(string motivo, DateTime fechaInicio, DateTime fechaFin)
        {
            var user = GetCurrentUser();
            if (user == null || !PlanService.CanAccessReservas(user.Rol?.Nombre ?? "Basico")) return RedirectToAction("Reservas");
            new CADReserva().CrearReserva(new ENReserva
            {
                Motivo      = motivo,
                FechaInicio = fechaInicio,
                FechaFin    = fechaFin,
                Estado      = "confirmada",
                UsuarioId   = user.Id,
                ZonaComunId = 1
            });
            return RedirectToAction("Reservas");
        }

        [HttpPost]
        public IActionResult CancelReservation(int id)
        {
            new CADReserva().CancelarReserva(new ENReserva { Id = id });
            return RedirectToAction("Reservas");
        }

        // ═══ INCIDENCIAS ═════════════════════════════════════════════
        public IActionResult Incidencias()
        {
            var r = RequireLogin(); if (r != null) return r;
            var user = GetCurrentUser()!;
            ViewBag.UserRole = user.Rol?.Nombre ?? "Basico";
            if (!PlanService.CanAccessIncidencias(user.Rol?.Nombre ?? "Basico"))
            {
                ViewBag.Locked = true;
                return View(new List<ENIncidencia>());
            }
            var incidents = new CADIncidencia().ListarTodas(GetUserPisoId());
            return View(incidents);
        }

        [HttpPost]
        public IActionResult ReportIncident(string titulo, string descripcion)
        {
            var user = GetCurrentUser();
            if (user == null || !PlanService.CanAccessIncidencias(user.Rol?.Nombre ?? "Basico")) return RedirectToAction("Incidencias");
            new CADIncidencia().CrearIncidencia(new ENIncidencia
            {
                Titulo         = titulo,
                Descripcion    = descripcion,
                Estado         = "abierta",
                FechaReporte   = DateTime.Now,
                ReportadaPorId = user.Id,
                PisoId         = GetUserPisoId()
            });
            return RedirectToAction("Incidencias");
        }

        [HttpPost]
        public IActionResult UpdateIncidentStatus(int id, string status)
        {
            new CADIncidencia().ActualizarEstado(id, status);
            return RedirectToAction("Incidencias");
        }

        // ═══ CONTRATOS Y PAGOS ═══════════════════════════════════════
        public IActionResult ContratosYPagos()
        {
            var r = RequireLogin(); if (r != null) return r;
            var user = GetCurrentUser()!;
            ViewBag.UserRole = user.Rol?.Nombre ?? "Basico";
            if (!PlanService.CanAccessContratos(user.Rol?.Nombre ?? "Basico"))
            {
                ViewBag.Locked = true;
                return View(new ContratosViewModel());
            }
            var contratos = new CADContrato().ListarTodos(user.Id);
            var pagos     = new CADPago().ListarTodos();
            return View(new ContratosViewModel { Contratos = contratos, Pagos = pagos });
        }

        [HttpPost]
        public IActionResult AddContrato(string type, DateTime startDate, DateTime endDate, decimal monthlyRent, decimal depositAmount)
        {
            var user = GetCurrentUser();
            if (user == null || !PlanService.CanAccessContratos(user.Rol?.Nombre ?? "Basico")) return RedirectToAction("ContratosYPagos");
            new CADContrato().CrearContrato(new ENContrato
            {
                Type          = type,
                StartDate     = startDate,
                EndDate       = endDate,
                MonthlyRent   = monthlyRent,
                DepositAmount = depositAmount,
                Status        = "activo",
                PropertyId    = GetUserPisoId(),
                UserId        = user.Id
            });
            return RedirectToAction("ContratosYPagos");
        }

        [HttpPost]
        public IActionResult AddPago(int contratoId, decimal amount, string method, string concept)
        {
            var user = GetCurrentUser();
            if (user == null || !PlanService.CanAccessContratos(user.Rol?.Nombre ?? "Basico")) return RedirectToAction("ContratosYPagos");
            new CADPago().CrearPago(new ENPago
            {
                Amount     = amount,
                Date       = DateTime.Now,
                Method     = method,
                Status     = "pagado",
                Concept    = concept,
                ContratoId = contratoId,
                UserId     = user.Id
            });
            return RedirectToAction("ContratosYPagos");
        }

        [HttpPost]
        public IActionResult DeleteContrato(int id)
        {
            new CADContrato().BorrarContrato(new ENContrato { Id = id });
            return RedirectToAction("ContratosYPagos");
        }

        // ═══ DOCUMENTOS ══════════════════════════════════════════════
        public IActionResult Documentos()
        {
            var r = RequireLogin(); if (r != null) return r;
            var user = GetCurrentUser()!;
            ViewBag.UserRole = user.Rol?.Nombre ?? "Basico";
            if (!PlanService.CanUploadDocuments(user.Rol?.Nombre ?? "Basico"))
            {
                ViewBag.Locked = true;
                return View(new List<ENDocumento>());
            }
            var docs = new CADDocumento().ListarPorUsuario(user.Id);
            return View(docs);
        }

        [HttpPost]
        public IActionResult UploadDocumento(IFormFile file, string type, string description)
        {
            var user = GetCurrentUser();
            if (user == null || !PlanService.CanUploadDocuments(user.Rol?.Nombre ?? "Basico") || file == null)
                return RedirectToAction("Documentos");

            using var ms = new MemoryStream();
            file.CopyTo(ms);
            new CADDocumento().CrearDocumento(new ENDocumento
            {
                FileName    = file.FileName,
                FileData    = ms.ToArray(),
                ContentType = file.ContentType,
                FileSize    = file.Length,
                Type        = type ?? "otro",
                Description = description,
                UploadDate  = DateTime.Now,
                PropertyId  = GetUserPisoId(),
                UserId      = user.Id
            });
            return RedirectToAction("Documentos");
        }

        public IActionResult DownloadDocumento(int id)
        {
            var doc = new CADDocumento().LeerDocumento(id);
            if (doc == null || doc.FileData == null) return NotFound();
            return File(doc.FileData, doc.ContentType ?? "application/octet-stream", doc.FileName);
        }

        [HttpPost]
        public IActionResult DeleteDocumento(int id)
        {
            new CADDocumento().BorrarDocumento(new ENDocumento { Id = id });
            return RedirectToAction("Documentos");
        }

        // ═══ MIS PISOS ═══════════════════════════════════════════════
        public IActionResult MisPisos()
        {
            var r = RequireLogin(); if (r != null) return r;
            var user = GetCurrentUser()!;
            ViewBag.UserRole = user.Rol?.Nombre ?? "Basico";
            if (!PlanService.CanManageProperties(user.Rol?.Nombre ?? "Basico"))
            {
                ViewBag.Locked = true;
                return View(new List<ENPiso>());
            }
            var pisos = new CADPiso().ListarTodos();
            return View(pisos);
        }

        [HttpPost]
        public IActionResult AddProperty(string direccion, string ciudad, int habitaciones, decimal precioTotal, string descripcion)
        {
            var user = GetCurrentUser();
            if (user == null || !PlanService.CanManageProperties(user.Rol?.Nombre ?? "Basico")) return RedirectToAction("MisPisos");

            var pisos = new CADPiso().ListarTodos();
            if (pisos.Count >= PlanService.MaxProperties(user.Rol?.Nombre ?? "Basico"))
            {
                TempData["Error"] = $"Has alcanzado el límite de pisos para tu plan ({PlanService.DisplayName(user.Rol?.Nombre ?? "Basico")})";
                return RedirectToAction("MisPisos");
            }

            new CADPiso().CrearPiso(new ENPiso
            {
                Direccion           = direccion,
                Ciudad              = ciudad,
                NumeroHabitaciones  = habitaciones,
                PrecioTotal         = precioTotal,
                Descripcion         = descripcion,
                Disponible          = true
            });
            return RedirectToAction("MisPisos");
        }
    }

    // ViewModel para la página de Contratos
    public class ContratosViewModel
    {
        public List<ENContrato> Contratos { get; set; } = new();
        public List<ENPago>     Pagos     { get; set; } = new();
    }
}
