using System;
using System.Collections.Generic;
using System.Linq;

namespace ConviAppWeb.Models
{
    /// <summary>CADSuscripcion — Clase de Acceso a Datos para suscripciones (Moni).</summary>
    public class CADSuscripcion
    {
        private readonly ApplicationDbContext _context;
        public CADSuscripcion(ApplicationDbContext context) => _context = context;

        public ENSuscripcion Create(ENSuscripcion s) { _context.Suscripciones.Add(s); _context.SaveChanges(); return s; }
        public ENSuscripcion ReadById(int id) => _context.Suscripciones.Find(id);
        public List<ENSuscripcion> ReadAll() => _context.Suscripciones.ToList();
        public List<ENSuscripcion> ReadActivas() => _context.Suscripciones.Where(s => s.Activa && s.FechaFin >= DateTime.Now).ToList();
        public ENSuscripcion ReadByUsuario(int usuarioId) => _context.Suscripciones.FirstOrDefault(s => s.UsuarioId == usuarioId && s.Activa);

        public ENSuscripcion Update(ENSuscripcion s)
        {
            var ex = _context.Suscripciones.Find(s.Id);
            if (ex == null) return null;
            ex.Plan = s.Plan; ex.PrecioMensual = s.PrecioMensual;
            ex.FechaFin = s.FechaFin; ex.Activa = s.Activa;
            _context.SaveChanges();
            return ex;
        }

        public bool Delete(int id)
        {
            var s = _context.Suscripciones.Find(id);
            if (s == null) return false;
            _context.Suscripciones.Remove(s); _context.SaveChanges(); return true;
        }
    }
}
