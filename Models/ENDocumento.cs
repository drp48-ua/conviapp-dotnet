using System;
using System.ComponentModel.DataAnnotations;

namespace ConviAppWeb.Models
{
    /// <summary>
    /// ENDocumento — Entidad de Negocio para documentos adjuntos.
    /// Capa de lógica de negocio (Entrega 3 - Dani).
    /// </summary>
    public class ENDocumento
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del archivo es obligatorio")]
        public string FileName { get; set; }

        [Required]
        public byte[] FileData { get; set; }

        [Required]
        public string ContentType { get; set; } // application/pdf, image/png, etc.

        public long FileSize { get; set; }

        [Required]
        public string Type { get; set; } = "otro"; // contrato, recibo, factura, dni, otro

        public DateTime UploadDate { get; set; } = DateTime.Now;

        public string? Description { get; set; }

        // FK → Contrato (optional)
        public int? ContratoId { get; set; }
        public ENContrato Contrato { get; set; }

        // FK → Property (optional)
        public int? PropertyId { get; set; }
        public Property Property { get; set; }

        // FK → User (who uploaded)
        public int UserId { get; set; }
        public User User { get; set; }

        // ─── Business Logic Methods ───
        public string FileSizeFormatted()
        {
            if (FileSize < 1024) return $"{FileSize} B";
            if (FileSize < 1048576) return $"{FileSize / 1024.0:F1} KB";
            return $"{FileSize / 1048576.0:F1} MB";
        }

        public bool IsPdf() => ContentType == "application/pdf";
        public bool IsImage() => ContentType?.StartsWith("image/") == true;
    }
}
