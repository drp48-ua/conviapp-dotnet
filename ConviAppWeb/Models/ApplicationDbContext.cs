using Microsoft.EntityFrameworkCore;

namespace ConviAppWeb.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ══════════════════════════════════════════════════════════
        // ENTIDADES LEGACY (ya existían en el proyecto)
        // ══════════════════════════════════════════════════════════
        public DbSet<User> Users { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Incident> Incidents { get; set; }

        // ══════════════════════════════════════════════════════════
        // BLOQUE 1 — Usuario, Rol, Suscripción (Moni)
        // ══════════════════════════════════════════════════════════
        public DbSet<ENUsuario> Usuarios { get; set; }
        public DbSet<ENRol> Roles { get; set; }
        public DbSet<ENSuscripcion> Suscripciones { get; set; }

        // ══════════════════════════════════════════════════════════
        // BLOQUE 2 — Piso, Habitación, Imagen, Favorito (Marina)
        // ══════════════════════════════════════════════════════════
        public DbSet<ENPiso> Pisos { get; set; }
        public DbSet<ENHabitacion> Habitaciones { get; set; }
        public DbSet<ENImagen> Imagenes { get; set; }
        public DbSet<ENFavorito> Favoritos { get; set; }

        // ══════════════════════════════════════════════════════════
        // BLOQUE 3 — Contrato, Pago, Documento (Dani)
        // ══════════════════════════════════════════════════════════
        public DbSet<ENContrato> Contratos { get; set; }
        public DbSet<ENPago> Pagos { get; set; }
        public DbSet<ENDocumento> Documentos { get; set; }

        // ══════════════════════════════════════════════════════════
        // BLOQUE 4 — Tarea, Mensaje (Maca)
        // ══════════════════════════════════════════════════════════
        public DbSet<ENTarea> Tareas { get; set; }
        public DbSet<ENMensaje> Mensajes { get; set; }

        // ══════════════════════════════════════════════════════════
        // BLOQUE 5 — Gasto, CategoriaGasto, Incidencia (Nazim)
        // ══════════════════════════════════════════════════════════
        public DbSet<ENGasto> Gastos { get; set; }
        public DbSet<ENCategoriaGasto> CategoriasGasto { get; set; }
        public DbSet<ENIncidencia> Incidencias { get; set; }

        // ══════════════════════════════════════════════════════════
        // BLOQUE 6 — Reserva, ZonaComun, Notificacion (Lidia)
        // ══════════════════════════════════════════════════════════
        public DbSet<ENReserva> Reservas { get; set; }
        public DbSet<ENZonaComun> ZonasComunes { get; set; }
        public DbSet<ENNotificacion> Notificaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Índice único en email (User legacy) ──
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            // ── Índice único en email (ENUsuario) ──
            modelBuilder.Entity<ENUsuario>().HasIndex(u => u.Email).IsUnique();

            // ── Precisión decimal ──
            modelBuilder.Entity<ENContrato>().Property(c => c.MonthlyRent).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<ENContrato>().Property(c => c.DepositAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<ENContrato>().Property(c => c.CommissionRate).HasColumnType("decimal(5,2)");
            modelBuilder.Entity<ENPago>().Property(p => p.Amount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<ENGasto>().Property(g => g.Importe).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<ENHabitacion>().Property(h => h.Precio).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<ENSuscripcion>().Property(s => s.PrecioMensual).HasColumnType("decimal(18,2)");

            // ── ENUsuario → ENRol ──
            modelBuilder.Entity<ENUsuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── ENSuscripcion → ENUsuario ──
            modelBuilder.Entity<ENSuscripcion>()
                .HasOne(s => s.Usuario)
                .WithMany()
                .HasForeignKey(s => s.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── ENHabitacion → ENPiso ──
            modelBuilder.Entity<ENHabitacion>()
                .HasOne(h => h.Piso)
                .WithMany(p => p.Habitaciones)
                .HasForeignKey(h => h.PisoId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── ENImagen → ENHabitacion (optional) ──
            modelBuilder.Entity<ENImagen>()
                .HasOne(i => i.Habitacion)
                .WithMany(h => h.Imagenes)
                .HasForeignKey(i => i.HabitacionId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── ENImagen → ENPiso (optional) ──
            modelBuilder.Entity<ENImagen>()
                .HasOne(i => i.Piso)
                .WithMany()
                .HasForeignKey(i => i.PisoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── ENFavorito → ENUsuario ──
            modelBuilder.Entity<ENFavorito>()
                .HasOne(f => f.Usuario)
                .WithMany()
                .HasForeignKey(f => f.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── ENContrato → ENPago (cascade delete pagos) ──
            modelBuilder.Entity<ENPago>()
                .HasOne(p => p.Contrato)
                .WithMany(c => c.Pagos)
                .HasForeignKey(p => p.ContratoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── ENContrato → ENDocumento ──
            modelBuilder.Entity<ENDocumento>()
                .HasOne(d => d.Contrato)
                .WithMany(c => c.Documentos)
                .HasForeignKey(d => d.ContratoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── ENPago → User (legacy) ──
            modelBuilder.Entity<ENPago>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── ENContrato → User (legacy) ──
            modelBuilder.Entity<ENContrato>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── ENContrato → Property (legacy) ──
            modelBuilder.Entity<ENContrato>()
                .HasOne(c => c.Property)
                .WithMany()
                .HasForeignKey(c => c.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── ENDocumento → User (legacy) ──
            modelBuilder.Entity<ENDocumento>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── ENTarea — emisor/receptor usan ENUsuario ──
            modelBuilder.Entity<ENTarea>()
                .HasOne(t => t.CreadaPor)
                .WithMany()
                .HasForeignKey(t => t.CreadaPorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ENTarea>()
                .HasOne(t => t.AsignadaA)
                .WithMany()
                .HasForeignKey(t => t.AsignadaAId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── ENMensaje — emisor/receptor ──
            modelBuilder.Entity<ENMensaje>()
                .HasOne(m => m.Emisor)
                .WithMany()
                .HasForeignKey(m => m.EmisorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ENMensaje>()
                .HasOne(m => m.Receptor)
                .WithMany()
                .HasForeignKey(m => m.ReceptorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── ENGasto → ENCategoriaGasto ──
            modelBuilder.Entity<ENGasto>()
                .HasOne(g => g.Categoria)
                .WithMany(c => c.Gastos)
                .HasForeignKey(g => g.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── ENReserva → ENZonaComun ──
            modelBuilder.Entity<ENReserva>()
                .HasOne(r => r.ZonaComun)
                .WithMany(z => z.Reservas)
                .HasForeignKey(r => r.ZonaComunId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── ENReserva → ENUsuario ──
            modelBuilder.Entity<ENReserva>()
                .HasOne(r => r.Usuario)
                .WithMany()
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // ── ENNotificacion → ENUsuario ──
            modelBuilder.Entity<ENNotificacion>()
                .HasOne(n => n.Usuario)
                .WithMany()
                .HasForeignKey(n => n.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
