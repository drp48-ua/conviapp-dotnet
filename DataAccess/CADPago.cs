using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ConviAppWeb.Models
{
    /// <summary>
    /// CADPago — Clase de Acceso a Datos para pagos.
    /// Capa de acceso a datos (Entrega 3 - Dani).
    /// </summary>
    public class CADPago
    {
        private readonly ApplicationDbContext _context;

        public CADPago(ApplicationDbContext context)
        {
            _context = context;
        }

        // CREATE
        public ENPago Create(ENPago pago)
        {
            _context.Pagos.Add(pago);
            _context.SaveChanges();
            return pago;
        }

        // READ by Id
        public ENPago ReadById(int id)
        {
            return _context.Pagos
                .Include(p => p.Contrato)
                .Include(p => p.User)
                .FirstOrDefault(p => p.Id == id);
        }

        // READ all
        public List<ENPago> ReadAll()
        {
            return _context.Pagos
                .Include(p => p.Contrato)
                .Include(p => p.User)
                .OrderByDescending(p => p.Date)
                .ToList();
        }

        // READ by Contrato
        public List<ENPago> ReadByContrato(int contratoId)
        {
            return _context.Pagos
                .Include(p => p.User)
                .Where(p => p.ContratoId == contratoId)
                .OrderByDescending(p => p.Date)
                .ToList();
        }

        // READ by User
        public List<ENPago> ReadByUser(int userId)
        {
            return _context.Pagos
                .Include(p => p.Contrato)
                    .ThenInclude(c => c.Property)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.Date)
                .ToList();
        }

        // READ pending payments
        public List<ENPago> ReadPending()
        {
            return _context.Pagos
                .Include(p => p.Contrato)
                .Include(p => p.User)
                .Where(p => p.Status == "pendiente")
                .OrderBy(p => p.Date)
                .ToList();
        }

        // UPDATE
        public ENPago Update(ENPago pago)
        {
            var existing = _context.Pagos.Find(pago.Id);
            if (existing == null) return null;

            existing.Amount = pago.Amount;
            existing.Date = pago.Date;
            existing.Method = pago.Method;
            existing.Status = pago.Status;
            existing.Concept = pago.Concept;
            existing.Reference = pago.Reference;

            _context.SaveChanges();
            return existing;
        }

        // DELETE
        public bool Delete(int id)
        {
            var pago = _context.Pagos.Find(id);
            if (pago == null) return false;

            _context.Pagos.Remove(pago);
            _context.SaveChanges();
            return true;
        }

        // SUM total paid for a contract
        public decimal TotalPaidForContrato(int contratoId)
        {
            return _context.Pagos
                .Where(p => p.ContratoId == contratoId && p.Status == "pagado")
                .Sum(p => p.Amount);
        }
    }
}
