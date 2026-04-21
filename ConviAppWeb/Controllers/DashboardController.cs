using Microsoft.AspNetCore.Mvc;
using ConviAppWeb.Models;
using ConviAppWeb.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;

namespace ConviAppWeb.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        private User GetCurrentUser()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (email == null) return null;
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        private IActionResult RequireLogin()
        {
            if (GetCurrentUser() == null)
                return RedirectToAction("Login", "Account");
            return null;
        }

        // ═══ RESUMEN ═══
        public IActionResult Index()
        {
            var r = RequireLogin(); if (r != null) return r;
            var user = GetCurrentUser();
            ViewBag.UserRole = user.Role;
            ViewBag.UserName = user.Name ?? user.Email;
            var property = user.PropertyId.HasValue
                ? _context.Properties.Include(p => p.Tenants).FirstOrDefault(p => p.Id == user.PropertyId)
                : _context.Properties.Include(p => p.Tenants).FirstOrDefault();
            return View(property);
        }

        // ═══ TAREAS ═══
        public IActionResult Tareas()
        {
            var r = RequireLogin(); if (r != null) return r;
            ViewBag.UserRole = GetCurrentUser().Role;
            var user = GetCurrentUser();
            var tasks = _context.TaskItems.Include(t => t.Assignee)
                .Where(t => user.PropertyId == null || t.PropertyId == user.PropertyId)
                .ToList();
            return View(tasks);
        }

        [HttpPost]
        public IActionResult AddTask(string title, string description, string priority)
        {
            var user = GetCurrentUser();
            if (user == null) return RedirectToAction("Login", "Account");
            _context.TaskItems.Add(new TaskItem
            {
                Title = title,
                Description = description,
                Priority = priority ?? "Media",
                PropertyId = user.PropertyId ?? 1,
                AssigneeId = user.Id
            });
            _context.SaveChanges();
            return RedirectToAction("Tareas");
        }

        [HttpPost]
        public IActionResult CompleteTask(int id)
        {
            var task = _context.TaskItems.Find(id);
            if (task != null) { task.IsCompleted = !task.IsCompleted; _context.SaveChanges(); }
            return RedirectToAction("Tareas");
        }

        [HttpPost]
        public IActionResult DeleteTask(int id)
        {
            var task = _context.TaskItems.Find(id);
            if (task != null) { _context.TaskItems.Remove(task); _context.SaveChanges(); }
            return RedirectToAction("Tareas");
        }

        // ═══ GASTOS ═══
        public IActionResult Gastos()
        {
            var r = RequireLogin(); if (r != null) return r;
            ViewBag.UserRole = GetCurrentUser().Role;
            var user = GetCurrentUser();
            var expenses = _context.Expenses.Include(e => e.Payer)
                .Where(e => user.PropertyId == null || e.PropertyId == user.PropertyId)
                .OrderByDescending(e => e.Date)
                .ToList();
            return View(expenses);
        }

        [HttpPost]
        public IActionResult AddExpense(string description, decimal amount)
        {
            var user = GetCurrentUser();
            if (user == null) return RedirectToAction("Login", "Account");
            _context.Expenses.Add(new Expense
            {
                Description = description,
                Amount = amount,
                Date = DateTime.Now,
                PayerId = user.Id,
                PropertyId = user.PropertyId ?? 1
            });
            _context.SaveChanges();
            return RedirectToAction("Gastos");
        }

        [HttpPost]
        public IActionResult DeleteExpense(int id)
        {
            var exp = _context.Expenses.Find(id);
            if (exp != null) { _context.Expenses.Remove(exp); _context.SaveChanges(); }
            return RedirectToAction("Gastos");
        }

        // ═══ MENSAJES ═══
        public IActionResult Mensajes()
        {
            var r = RequireLogin(); if (r != null) return r;
            ViewBag.UserRole = GetCurrentUser().Role;
            var user = GetCurrentUser();
            var messages = _context.Messages.Include(m => m.Sender)
                .Where(m => user.PropertyId == null || m.PropertyId == user.PropertyId)
                .OrderBy(m => m.Timestamp)
                .ToList();
            return View(messages);
        }

        [HttpPost]
        public IActionResult SendMessage(string text)
        {
            var user = GetCurrentUser();
            if (user == null || string.IsNullOrEmpty(text)) return RedirectToAction("Mensajes");
            _context.Messages.Add(new Message
            {
                Text = text,
                Timestamp = DateTime.Now,
                SenderId = user.Id,
                PropertyId = user.PropertyId ?? 1
            });
            _context.SaveChanges();
            return RedirectToAction("Mensajes");
        }

        // ═══ RESERVAS ═══
        public IActionResult Reservas()
        {
            var r = RequireLogin(); if (r != null) return r;
            var user = GetCurrentUser();
            ViewBag.UserRole = user.Role;
            if (!PlanService.CanAccessReservas(user.Role))
            {
                ViewBag.Locked = true;
                return View(new System.Collections.Generic.List<Reservation>());
            }
            var reservations = _context.Reservations.Include(rv => rv.User)
                .Where(rv => user.PropertyId == null || rv.PropertyId == user.PropertyId)
                .OrderBy(rv => rv.StartTime)
                .ToList();
            return View(reservations);
        }

        [HttpPost]
        public IActionResult AddReservation(string commonArea, DateTime startTime, DateTime endTime)
        {
            var user = GetCurrentUser();
            if (user == null || !PlanService.CanAccessReservas(user.Role)) return RedirectToAction("Reservas");
            _context.Reservations.Add(new Reservation
            {
                CommonArea = commonArea,
                StartTime = startTime,
                EndTime = endTime,
                UserId = user.Id,
                PropertyId = user.PropertyId ?? 1
            });
            _context.SaveChanges();
            return RedirectToAction("Reservas");
        }

        [HttpPost]
        public IActionResult CancelReservation(int id)
        {
            var res = _context.Reservations.Find(id);
            if (res != null) { _context.Reservations.Remove(res); _context.SaveChanges(); }
            return RedirectToAction("Reservas");
        }

        // ═══ INCIDENCIAS ═══
        public IActionResult Incidencias()
        {
            var r = RequireLogin(); if (r != null) return r;
            var user = GetCurrentUser();
            ViewBag.UserRole = user.Role;
            if (!PlanService.CanAccessIncidencias(user.Role))
            {
                ViewBag.Locked = true;
                return View(new System.Collections.Generic.List<Incident>());
            }
            var incidents = _context.Incidents
                .Where(i => user.PropertyId == null || i.PropertyId == user.PropertyId)
                .OrderByDescending(i => i.DateReported)
                .ToList();
            return View(incidents);
        }

        [HttpPost]
        public IActionResult ReportIncident(string title, string description)
        {
            var user = GetCurrentUser();
            if (user == null || !PlanService.CanAccessIncidencias(user.Role)) return RedirectToAction("Incidencias");
            _context.Incidents.Add(new Incident
            {
                Title = title,
                Description = description,
                Status = "abierta",
                DateReported = DateTime.Now,
                PropertyId = user.PropertyId ?? 1
            });
            _context.SaveChanges();
            return RedirectToAction("Incidencias");
        }

        [HttpPost]
        public IActionResult UpdateIncidentStatus(int id, string status)
        {
            var inc = _context.Incidents.Find(id);
            if (inc != null) { inc.Status = status; _context.SaveChanges(); }
            return RedirectToAction("Incidencias");
        }

        // ═══ CONTRATOS Y PAGOS (EN/CAD — Entrega 3 Dani) ═══
        public IActionResult ContratosYPagos()
        {
            var r = RequireLogin(); if (r != null) return r;
            var user = GetCurrentUser();
            ViewBag.UserRole = user.Role;
            if (!PlanService.CanAccessContratos(user.Role))
            {
                ViewBag.Locked = true;
                return View(new ContratosViewModel());
            }
            var cadContrato = new CADContrato(_context);
            var cadPago = new CADPago(_context);
            var contratos = cadContrato.ReadByUser(user.Id);
            var pagos = cadPago.ReadByUser(user.Id);
            return View(new ContratosViewModel { Contratos = contratos, Pagos = pagos });
        }

        [HttpPost]
        public IActionResult AddContrato(string type, DateTime startDate, DateTime endDate, decimal monthlyRent, decimal depositAmount)
        {
            var user = GetCurrentUser();
            if (user == null || !PlanService.CanAccessContratos(user.Role)) return RedirectToAction("ContratosYPagos");
            var cadContrato = new CADContrato(_context);
            cadContrato.Create(new ENContrato
            {
                Type = type,
                StartDate = startDate,
                EndDate = endDate,
                MonthlyRent = monthlyRent,
                DepositAmount = depositAmount,
                Status = "activo",
                PropertyId = user.PropertyId ?? 1,
                UserId = user.Id
            });
            return RedirectToAction("ContratosYPagos");
        }

        [HttpPost]
        public IActionResult AddPago(int contratoId, decimal amount, string method, string concept)
        {
            var user = GetCurrentUser();
            if (user == null || !PlanService.CanAccessContratos(user.Role)) return RedirectToAction("ContratosYPagos");
            var cadPago = new CADPago(_context);
            cadPago.Create(new ENPago
            {
                Amount = amount,
                Date = DateTime.Now,
                Method = method,
                Status = "pagado",
                Concept = concept,
                ContratoId = contratoId,
                UserId = user.Id
            });
            return RedirectToAction("ContratosYPagos");
        }

        [HttpPost]
        public IActionResult DeleteContrato(int id)
        {
            var cadContrato = new CADContrato(_context);
            cadContrato.Delete(id);
            return RedirectToAction("ContratosYPagos");
        }

        // ═══ DOCUMENTOS ═══
        public IActionResult Documentos()
        {
            var r = RequireLogin(); if (r != null) return r;
            var user = GetCurrentUser();
            ViewBag.UserRole = user.Role;
            if (!PlanService.CanUploadDocuments(user.Role))
            {
                ViewBag.Locked = true;
                return View(new System.Collections.Generic.List<ENDocumento>());
            }
            var cadDoc = new CADDocumento(_context);
            var docs = cadDoc.ReadByUser(user.Id);
            return View(docs);
        }

        [HttpPost]
        public IActionResult UploadDocumento(IFormFile file, string type, string description)
        {
            var user = GetCurrentUser();
            if (user == null || !PlanService.CanUploadDocuments(user.Role) || file == null) return RedirectToAction("Documentos");

            using var ms = new MemoryStream();
            file.CopyTo(ms);

            var cadDoc = new CADDocumento(_context);
            cadDoc.Create(new ENDocumento
            {
                FileName = file.FileName,
                FileData = ms.ToArray(),
                ContentType = file.ContentType,
                FileSize = file.Length,
                Type = type ?? "otro",
                Description = description,
                UploadDate = DateTime.Now,
                PropertyId = user.PropertyId,
                UserId = user.Id
            });
            return RedirectToAction("Documentos");
        }

        public IActionResult DownloadDocumento(int id)
        {
            var cadDoc = new CADDocumento(_context);
            var doc = cadDoc.Download(id);
            if (doc == null || doc.FileData == null) return NotFound();
            return File(doc.FileData, doc.ContentType, doc.FileName);
        }

        [HttpPost]
        public IActionResult DeleteDocumento(int id)
        {
            var cadDoc = new CADDocumento(_context);
            cadDoc.Delete(id);
            return RedirectToAction("Documentos");
        }

        // ═══ MIS PISOS ═══
        public IActionResult MisPisos()
        {
            var r = RequireLogin(); if (r != null) return r;
            var user = GetCurrentUser();
            ViewBag.UserRole = user.Role;
            if (!PlanService.CanManageProperties(user.Role))
            {
                ViewBag.Locked = true;
                return View(new System.Collections.Generic.List<Property>());
            }
            var properties = _context.Properties.Include(p => p.Tenants).ToList();
            return View(properties);
        }

        [HttpPost]
        public IActionResult AddProperty(string name, string location, int rooms, decimal price, string description)
        {
            var user = GetCurrentUser();
            if (user == null || !PlanService.CanManageProperties(user.Role)) return RedirectToAction("MisPisos");

            var count = _context.Properties.Count();
            if (count >= PlanService.MaxProperties(user.Role))
            {
                TempData["Error"] = $"Has alcanzado el límite de pisos para tu plan ({PlanService.DisplayName(user.Role)})";
                return RedirectToAction("MisPisos");
            }

            _context.Properties.Add(new Property
            {
                Name = name,
                Location = location,
                Rooms = rooms,
                Price = price,
                Description = description
            });
            _context.SaveChanges();
            return RedirectToAction("MisPisos");
        }
    }

    // ViewModel for Contratos page
    public class ContratosViewModel
    {
        public System.Collections.Generic.List<ENContrato> Contratos { get; set; } = new();
        public System.Collections.Generic.List<ENPago> Pagos { get; set; } = new();
    }
}
