using ConviAppWeb.Models;
using ConviAppWeb.Services;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Email service
builder.Services.AddSingleton<EmailService>();

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();

    // Seed Properties
    if (!db.Properties.Any())
    {
        var prop1 = new Property { Name = "Piso Centro Madrid", Description = "Amplio piso en el corazón de Madrid con 4 habitaciones", Location = "Calle Gran Vía 42, Madrid", Price = 1200, Rooms = 4, ImageFile = "piso_madrid.jpg" };
        var prop2 = new Property { Name = "Apartamento Playa Valencia", Description = "Apartamento luminoso cerca de la playa de la Malvarrosa", Location = "Av. Malvarrosa 15, Valencia", Price = 850, Rooms = 3, ImageFile = "piso_valencia.jpg" };
        var prop3 = new Property { Name = "Piso Universitario Alicante", Description = "Piso ideal para estudiantes cerca de la UA", Location = "Calle San Vicente 88, Alicante", Price = 600, Rooms = 3, ImageFile = "piso_alicante.jpg" };
        db.Properties.AddRange(prop1, prop2, prop3);
        db.SaveChanges();

        // Seed Users
        var users = new[]
        {
            new User { Email = "dani@conviapp.com", Password = "1234", Name = "Dani", Role = "Enterprise", PropertyId = prop1.Id },
            new User { Email = "lidia@conviapp.com", Password = "1234", Name = "Lidia", Role = "Profesional", PropertyId = prop1.Id },
            new User { Email = "marina@conviapp.com", Password = "1234", Name = "Marina", Role = "Basico", PropertyId = prop2.Id },
            new User { Email = "nazim@conviapp.com", Password = "1234", Name = "Nazim", Role = "Basico", PropertyId = prop2.Id }
        };
        db.Users.AddRange(users);
        db.SaveChanges();

        // Seed Tasks
        db.TaskItems.AddRange(
            new TaskItem { Title = "Limpiar baño principal", Description = "Limpieza completa del baño", Priority = "Alta", PropertyId = prop1.Id, AssigneeId = users[0].Id },
            new TaskItem { Title = "Comprar papel higiénico", Description = "Comprar paquete de 12", Priority = "Media", PropertyId = prop1.Id, AssigneeId = users[1].Id },
            new TaskItem { Title = "Sacar la basura", Description = "Contenedores de orgánico y plástico", Priority = "Baja", IsCompleted = true, PropertyId = prop1.Id, AssigneeId = users[0].Id },
            new TaskItem { Title = "Fregar suelos", Description = "Fregado de cocina y pasillos", Priority = "Media", PropertyId = prop2.Id, AssigneeId = users[2].Id }
        );

        // Seed Expenses
        db.Expenses.AddRange(
            new Expense { Description = "Factura WiFi Abril", Amount = 45.00m, Date = DateTime.Now.AddDays(-5), PayerId = users[0].Id, PropertyId = prop1.Id },
            new Expense { Description = "Compra supermercado", Amount = 87.50m, Date = DateTime.Now.AddDays(-3), PayerId = users[1].Id, PropertyId = prop1.Id },
            new Expense { Description = "Factura electricidad", Amount = 62.30m, Date = DateTime.Now.AddDays(-1), PayerId = users[0].Id, PropertyId = prop1.Id }
        );

        // Seed Messages
        db.Messages.AddRange(
            new Message { Text = "¡Bienvenidos al piso! 🏠", Timestamp = DateTime.Now.AddHours(-48), SenderId = users[0].Id, PropertyId = prop1.Id },
            new Message { Text = "He dejado las llaves en la entrada", Timestamp = DateTime.Now.AddHours(-24), SenderId = users[1].Id, PropertyId = prop1.Id },
            new Message { Text = "¿Alguien puede comprar leche?", Timestamp = DateTime.Now.AddHours(-2), SenderId = users[0].Id, PropertyId = prop1.Id }
        );

        // Seed Reservations
        db.Reservations.AddRange(
            new Reservation { CommonArea = "Lavadora", StartTime = DateTime.Now.AddHours(2), EndTime = DateTime.Now.AddHours(4), UserId = users[0].Id, PropertyId = prop1.Id },
            new Reservation { CommonArea = "Salón (reunión)", StartTime = DateTime.Now.AddDays(1), EndTime = DateTime.Now.AddDays(1).AddHours(3), UserId = users[1].Id, PropertyId = prop1.Id }
        );

        // Seed Incidents
        db.Incidents.AddRange(
            new Incident { Title = "Grifo cocina gotea", Description = "El grifo de la cocina pierde agua constantemente", Status = "abierta", DateReported = DateTime.Now.AddDays(-7), PropertyId = prop1.Id },
            new Incident { Title = "Bombilla pasillo fundida", Description = "La luz del pasillo no funciona", Status = "en proceso", DateReported = DateTime.Now.AddDays(-3), PropertyId = prop1.Id },
            new Incident { Title = "Cerradura puerta principal", Description = "La cerradura está dura, cuesta cerrar", Status = "cerrada", DateReported = DateTime.Now.AddDays(-14), PropertyId = prop1.Id }
        );

        // Seed Contracts (ENContrato)
        var contrato1 = new ENContrato
        {
            Type = "arrendamiento",
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 12, 31),
            MonthlyRent = 400,
            DepositAmount = 800,
            Status = "activo",
            Notes = "Contrato de arrendamiento estándar",
            PropertyId = prop1.Id,
            UserId = users[0].Id
        };
        var contrato2 = new ENContrato
        {
            Type = "arrendamiento",
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 12, 31),
            MonthlyRent = 380,
            DepositAmount = 760,
            Status = "activo",
            Notes = "Contrato sujeto a revisión semestral",
            PropertyId = prop1.Id,
            UserId = users[1].Id
        };
        db.Contratos.AddRange(contrato1, contrato2);
        db.SaveChanges();

        // Seed Payments (ENPago)
        db.Pagos.AddRange(
            new ENPago { Amount = 400, Date = new DateTime(2026, 1, 5), Method = "transferencia", Status = "pagado", Concept = "Alquiler Enero 2026", ContratoId = contrato1.Id, UserId = users[0].Id },
            new ENPago { Amount = 400, Date = new DateTime(2026, 2, 5), Method = "transferencia", Status = "pagado", Concept = "Alquiler Febrero 2026", ContratoId = contrato1.Id, UserId = users[0].Id },
            new ENPago { Amount = 400, Date = new DateTime(2026, 3, 5), Method = "bizum", Status = "pagado", Concept = "Alquiler Marzo 2026", ContratoId = contrato1.Id, UserId = users[0].Id },
            new ENPago { Amount = 400, Date = new DateTime(2026, 4, 5), Method = "transferencia", Status = "pagado", Concept = "Alquiler Abril 2026", ContratoId = contrato1.Id, UserId = users[0].Id },
            new ENPago { Amount = 380, Date = new DateTime(2026, 1, 5), Method = "tarjeta", Status = "pagado", Concept = "Alquiler Enero 2026", ContratoId = contrato2.Id, UserId = users[1].Id },
            new ENPago { Amount = 380, Date = new DateTime(2026, 2, 5), Method = "tarjeta", Status = "pagado", Concept = "Alquiler Febrero 2026", ContratoId = contrato2.Id, UserId = users[1].Id },
            new ENPago { Amount = 380, Date = new DateTime(2026, 3, 5), Method = "tarjeta", Status = "pagado", Concept = "Alquiler Marzo 2026", ContratoId = contrato2.Id, UserId = users[1].Id },
            new ENPago { Amount = 380, Date = new DateTime(2026, 4, 5), Method = "transferencia", Status = "pendiente", Concept = "Alquiler Abril 2026", ContratoId = contrato2.Id, UserId = users[1].Id }
        );

        db.SaveChanges();
    }
}

// Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
