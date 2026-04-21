using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ConviAppWeb.Models
{
    /// <summary>
    /// CADDocumento — Clase de Acceso a Datos para documentos.
    /// Capa de acceso a datos (Entrega 3 - Dani).
    /// </summary>
    public class CADDocumento
    {
        private readonly ApplicationDbContext _context;

        public CADDocumento(ApplicationDbContext context)
        {
            _context = context;
        }

        // CREATE
        public ENDocumento Create(ENDocumento documento)
        {
            _context.Documentos.Add(documento);
            _context.SaveChanges();
            return documento;
        }

        // READ by Id
        public ENDocumento ReadById(int id)
        {
            return _context.Documentos
                .Include(d => d.Contrato)
                .Include(d => d.Property)
                .Include(d => d.User)
                .FirstOrDefault(d => d.Id == id);
        }

        // READ all (without FileData for performance)
        public List<ENDocumento> ReadAll()
        {
            return _context.Documentos
                .Include(d => d.User)
                .Select(d => new ENDocumento
                {
                    Id = d.Id,
                    FileName = d.FileName,
                    ContentType = d.ContentType,
                    FileSize = d.FileSize,
                    Type = d.Type,
                    UploadDate = d.UploadDate,
                    Description = d.Description,
                    ContratoId = d.ContratoId,
                    PropertyId = d.PropertyId,
                    UserId = d.UserId,
                    User = d.User
                })
                .OrderByDescending(d => d.UploadDate)
                .ToList();
        }

        // READ by Property
        public List<ENDocumento> ReadByProperty(int propertyId)
        {
            return _context.Documentos
                .Include(d => d.User)
                .Where(d => d.PropertyId == propertyId)
                .OrderByDescending(d => d.UploadDate)
                .ToList();
        }

        // READ by Contrato
        public List<ENDocumento> ReadByContrato(int contratoId)
        {
            return _context.Documentos
                .Include(d => d.User)
                .Where(d => d.ContratoId == contratoId)
                .OrderByDescending(d => d.UploadDate)
                .ToList();
        }

        // READ by User
        public List<ENDocumento> ReadByUser(int userId)
        {
            return _context.Documentos
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.UploadDate)
                .ToList();
        }

        // DOWNLOAD (get full file data)
        public ENDocumento Download(int id)
        {
            return _context.Documentos.Find(id);
        }

        // UPDATE metadata (not the file itself)
        public ENDocumento Update(ENDocumento documento)
        {
            var existing = _context.Documentos.Find(documento.Id);
            if (existing == null) return null;

            existing.FileName = documento.FileName;
            existing.Type = documento.Type;
            existing.Description = documento.Description;

            _context.SaveChanges();
            return existing;
        }

        // DELETE
        public bool Delete(int id)
        {
            var documento = _context.Documentos.Find(id);
            if (documento == null) return false;

            _context.Documentos.Remove(documento);
            _context.SaveChanges();
            return true;
        }

        // COUNT by user
        public int CountByUser(int userId)
        {
            return _context.Documentos.Count(d => d.UserId == userId);
        }
    }
}
