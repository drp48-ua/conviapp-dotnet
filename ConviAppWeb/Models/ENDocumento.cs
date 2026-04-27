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
        // ─── Atributos privados ───
        private int _id;
        private string _fileName;
        private byte[] _fileData;
        private string _contentType;
        private long _fileSize;
        private string _type;
        private DateTime _uploadDate;
        private string _description;
        private int? _contratoId;
        private int? _propertyId;
        private int _userId;

        // ─── Propiedades públicas ───
        [Key]
        public int Id
        {
            get => _id;
            set => _id = value;
        }

        [Required(ErrorMessage = "El nombre del archivo es obligatorio")]
        public string FileName
        {
            get => _fileName;
            set => _fileName = value;
        }

        [Required]
        public byte[] FileData
        {
            get => _fileData;
            set => _fileData = value;
        }

        [Required]
        public string ContentType
        {
            get => _contentType;
            set => _contentType = value;
        } // application/pdf, image/png, etc.

        public long FileSize
        {
            get => _fileSize;
            set => _fileSize = value;
        }

        [Required]
        public string Type
        {
            get => _type;
            set => _type = value;
        } // contrato, recibo, factura, dni, otro

        public DateTime UploadDate
        {
            get => _uploadDate;
            set => _uploadDate = value;
        }

        public string? Description
        {
            get => _description;
            set => _description = value;
        }

        // ─── Claves foráneas ───
        public int? ContratoId
        {
            get => _contratoId;
            set => _contratoId = value;
        }
        public ENContrato? Contrato { get; set; }

        public int? PropertyId
        {
            get => _propertyId;
            set => _propertyId = value;
        }
        // Property nav eliminada (sin EF)

        public int UserId
        {
            get => _userId;
            set => _userId = value;
        }
        // User nav eliminada (sin EF)

        // ─── Métodos de negocio ───
        public string FileSizeFormatted()
        {
            if (_fileSize < 1024) return $"{_fileSize} B";
            if (_fileSize < 1048576) return $"{_fileSize / 1024.0:F1} KB";
            return $"{_fileSize / 1048576.0:F1} MB";
        }

        public bool IsPdf() => _contentType == "application/pdf";
        public bool IsImage() => _contentType?.StartsWith("image/") == true;

        /// <summary>Constructor por defecto.</summary>
        public ENDocumento()
        {
            _type = "otro";
            _uploadDate = DateTime.Now;
        }
    }
}
