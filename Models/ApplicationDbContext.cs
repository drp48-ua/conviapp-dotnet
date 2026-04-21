using Microsoft.EntityFrameworkCore;

namespace ConviAppWeb.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Existing entities
        public DbSet<User> Users { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Incident> Incidents { get; set; }

        // EN/CAD entities (Entrega 3 - Dani)
        public DbSet<ENContrato> Contratos { get; set; }
        public DbSet<ENPago> Pagos { get; set; }
        public DbSet<ENDocumento> Documentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User email unique index
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            // Decimal precision
            modelBuilder.Entity<ENContrato>().Property(c => c.MonthlyRent).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<ENContrato>().Property(c => c.DepositAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<ENPago>().Property(p => p.Amount).HasColumnType("decimal(18,2)");

            // ENContrato → Pagos cascade
            modelBuilder.Entity<ENPago>()
                .HasOne(p => p.Contrato)
                .WithMany(c => c.Pagos)
                .HasForeignKey(p => p.ContratoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ENContrato → Documentos cascade
            modelBuilder.Entity<ENDocumento>()
                .HasOne(d => d.Contrato)
                .WithMany(c => c.Documentos)
                .HasForeignKey(d => d.ContratoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ENPago → User
            modelBuilder.Entity<ENPago>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ENContrato → User
            modelBuilder.Entity<ENContrato>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ENContrato → Property
            modelBuilder.Entity<ENContrato>()
                .HasOne(c => c.Property)
                .WithMany()
                .HasForeignKey(c => c.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            // ENDocumento → User
            modelBuilder.Entity<ENDocumento>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
