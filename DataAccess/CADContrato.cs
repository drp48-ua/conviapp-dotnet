using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ConviAppWeb.Models
{
    /// <summary>
    /// CADContrato — Clase de Acceso a Datos para contratos.
    /// Capa de acceso a datos (Entrega 3 - Dani).
    /// Maneja las operaciones CRUD contra SQL Server.
    /// </summary>
    public class CADContrato
    {
        private readonly ApplicationDbContext _context;

        public CADContrato(ApplicationDbContext context)
        {
            _context = context;
        }

        // CREATE
        public ENContrato Create(ENContrato contrato)
        {
            _context.Contratos.Add(contrato);
            _context.SaveChanges();
            return contrato;
        }

        // READ by Id
        public ENContrato ReadById(int id)
        {
            return _context.Contratos
                .Include(c => c.Property)
                .Include(c => c.User)
                .Include(c => c.Pagos)
                .Include(c => c.Documentos)
                .FirstOrDefault(c => c.Id == id);
        }

        // READ all
        public List<ENContrato> ReadAll()
        {
            return _context.Contratos
                .Include(c => c.Property)
                .Include(c => c.User)
                .ToList();
        }

        // READ by Property
        public List<ENContrato> ReadByProperty(int propertyId)
        {
            return _context.Contratos
                .Include(c => c.User)
                .Include(c => c.Pagos)
                .Where(c => c.PropertyId == propertyId)
                .OrderByDescending(c => c.StartDate)
                .ToList();
        }

        // READ by User
        public List<ENContrato> ReadByUser(int userId)
        {
            return _context.Contratos
                .Include(c => c.Property)
                .Include(c => c.Pagos)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.StartDate)
                .ToList();
        }

        // UPDATE
        public ENContrato Update(ENContrato contrato)
        {
            var existing = _context.Contratos.Find(contrato.Id);
            if (existing == null) return null;

            existing.Type = contrato.Type;
            existing.StartDate = contrato.StartDate;
            existing.EndDate = contrato.EndDate;
            existing.MonthlyRent = contrato.MonthlyRent;
            existing.DepositAmount = contrato.DepositAmount;
            existing.Status = contrato.Status;
            existing.Notes = contrato.Notes;

            _context.SaveChanges();
            return existing;
        }

        // DELETE
        public bool Delete(int id)
        {
            var contrato = _context.Contratos.Find(id);
            if (contrato == null) return false;

            _context.Contratos.Remove(contrato);
            _context.SaveChanges();
            return true;
        }

        // COUNT active contracts
        public int CountActive()
        {
            return _context.Contratos.Count(c => c.Status == "activo");
        }
    }
}
