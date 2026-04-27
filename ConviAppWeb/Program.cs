using ConviAppWeb.Models;
using ConviAppWeb.Services;
using ConviAppWeb.DataAccess;
using System.Data.SQLite;

var builder = WebApplication.CreateBuilder(args);

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

// ═══ INICIALIZACIÓN DE BASE DE DATOS (ADO.NET) ═══════════════════════
InitDb();

// Configure pipeline
if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Home/Error");

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// ═══ MÉTODO DE INICIALIZACIÓN ════════════════════════════════════════
void InitDb()
{
    string constring = DbConfig.ConnectionString;

    using var c = new SQLiteConnection(constring);
    c.Open();

    // Crear tablas
    ExecuteNonQuery(c, @"
        CREATE TABLE IF NOT EXISTS Usuario (
            id             INTEGER PRIMARY KEY AUTOINCREMENT,
            nombre         TEXT,
            apellidos      TEXT,
            email          TEXT NOT NULL UNIQUE,
            password_hash  TEXT NOT NULL,
            telefono       TEXT,
            fecha_registro TEXT NOT NULL,
            activo         INTEGER NOT NULL DEFAULT 1,
            rol            TEXT NOT NULL DEFAULT 'Basico'
        );");

    ExecuteNonQuery(c, @"
        CREATE TABLE IF NOT EXISTS Piso (
            id                  INTEGER PRIMARY KEY AUTOINCREMENT,
            direccion           TEXT,
            ciudad              TEXT,
            codigo_postal       TEXT,
            numero_habitaciones INTEGER NOT NULL DEFAULT 0,
            numero_banos        INTEGER NOT NULL DEFAULT 0,
            precio_total        REAL NOT NULL DEFAULT 0,
            descripcion         TEXT,
            disponible          INTEGER NOT NULL DEFAULT 1
        );");


    ExecuteNonQuery(c, @"
        CREATE TABLE IF NOT EXISTS Tarea (
            id             INTEGER PRIMARY KEY AUTOINCREMENT,
            titulo         TEXT,
            descripcion    TEXT,
            estado         TEXT NOT NULL DEFAULT 'pendiente',
            prioridad      TEXT NOT NULL DEFAULT 'media',
            fecha_creacion TEXT NOT NULL,
            creada_por_id  INTEGER NOT NULL DEFAULT 0,
            piso_id        INTEGER
        );");

    ExecuteNonQuery(c, @"
        CREATE TABLE IF NOT EXISTS Gasto (
            id                INTEGER PRIMARY KEY AUTOINCREMENT,
            concepto          TEXT,
            importe           REAL NOT NULL DEFAULT 0,
            fecha             TEXT NOT NULL,
            pagado            INTEGER NOT NULL DEFAULT 0,
            descripcion       TEXT,
            registrado_por_id INTEGER NOT NULL DEFAULT 0,
            piso_id           INTEGER
        );");

    ExecuteNonQuery(c, @"
        CREATE TABLE IF NOT EXISTS Mensaje (
            id          INTEGER PRIMARY KEY AUTOINCREMENT,
            contenido   TEXT,
            fecha_envio TEXT NOT NULL,
            leido       INTEGER NOT NULL DEFAULT 0,
            emisor_id   INTEGER NOT NULL DEFAULT 0,
            piso_id     INTEGER
        );");

    ExecuteNonQuery(c, @"
        CREATE TABLE IF NOT EXISTS ZonaComun (
            id     INTEGER PRIMARY KEY AUTOINCREMENT,
            nombre TEXT,
            piso_id INTEGER
        );");

    ExecuteNonQuery(c, @"
        CREATE TABLE IF NOT EXISTS Reserva (
            id             INTEGER PRIMARY KEY AUTOINCREMENT,
            fecha_inicio   TEXT NOT NULL,
            fecha_fin      TEXT NOT NULL,
            estado         TEXT NOT NULL DEFAULT 'pendiente',
            motivo         TEXT,
            usuario_id     INTEGER NOT NULL DEFAULT 0,
            zona_comun_id  INTEGER NOT NULL DEFAULT 1
        );");

    ExecuteNonQuery(c, @"
        CREATE TABLE IF NOT EXISTS Incidencia (
            id                INTEGER PRIMARY KEY AUTOINCREMENT,
            titulo            TEXT,
            descripcion       TEXT,
            estado            TEXT NOT NULL DEFAULT 'abierta',
            prioridad         TEXT NOT NULL DEFAULT 'media',
            fecha_reporte     TEXT NOT NULL,
            reportada_por_id  INTEGER NOT NULL DEFAULT 0,
            piso_id           INTEGER
        );");

    ExecuteNonQuery(c, @"
        CREATE TABLE IF NOT EXISTS Contrato (
            id              INTEGER PRIMARY KEY AUTOINCREMENT,
            type            TEXT,
            start_date      TEXT NOT NULL,
            end_date        TEXT NOT NULL,
            monthly_rent    REAL NOT NULL DEFAULT 0,
            deposit_amount  REAL NOT NULL DEFAULT 0,
            status          TEXT NOT NULL DEFAULT 'activo',
            notes           TEXT,
            commission_rate REAL NOT NULL DEFAULT 0,
            property_id     INTEGER NOT NULL DEFAULT 1,
            user_id         INTEGER NOT NULL DEFAULT 0
        );");

    ExecuteNonQuery(c, @"
        CREATE TABLE IF NOT EXISTS Pago (
            id          INTEGER PRIMARY KEY AUTOINCREMENT,
            amount      REAL NOT NULL DEFAULT 0,
            date        TEXT NOT NULL,
            method      TEXT,
            status      TEXT NOT NULL DEFAULT 'pagado',
            concept     TEXT,
            reference   TEXT,
            contrato_id INTEGER NOT NULL DEFAULT 0,
            user_id     INTEGER NOT NULL DEFAULT 0
        );");

    ExecuteNonQuery(c, @"
        CREATE TABLE IF NOT EXISTS Documento (
            id           INTEGER PRIMARY KEY AUTOINCREMENT,
            file_name    TEXT,
            file_data    BLOB,
            content_type TEXT,
            file_size    INTEGER NOT NULL DEFAULT 0,
            type         TEXT NOT NULL DEFAULT 'otro',
            description  TEXT,
            upload_date  TEXT NOT NULL,
            property_id  INTEGER,
            user_id      INTEGER NOT NULL DEFAULT 0
        );");


    ExecuteNonQuery(c, @"
        CREATE TABLE IF NOT EXISTS Habitacion (
            id          INTEGER PRIMARY KEY AUTOINCREMENT,
            numero      TEXT,
            precio      REAL NOT NULL DEFAULT 0,
            metros      REAL NOT NULL DEFAULT 0,
            disponible  INTEGER NOT NULL DEFAULT 1,
            tiene_bano  INTEGER NOT NULL DEFAULT 0,
            descripcion TEXT,
            piso_id     INTEGER NOT NULL
        );");

    ExecuteNonQuery(c, @"
        CREATE TABLE IF NOT EXISTS Favorito (
            id            INTEGER PRIMARY KEY AUTOINCREMENT,
            fecha_guardado TEXT NOT NULL,
            usuario_id    INTEGER NOT NULL,
            habitacion_id INTEGER,
            piso_id       INTEGER
        );");

    ExecuteNonQuery(c, @"
        CREATE TABLE IF NOT EXISTS CategoriaGasto (
            id          INTEGER PRIMARY KEY AUTOINCREMENT,
            nombre      TEXT,
            descripcion TEXT,
            icono       TEXT
        );");

    ExecuteNonQuery(c, @"
        CREATE TABLE IF NOT EXISTS Imagen (
            id           INTEGER PRIMARY KEY AUTOINCREMENT,
            url          TEXT NOT NULL,
            descripcion  TEXT,
            es_principal INTEGER NOT NULL DEFAULT 0,
            fecha_subida TEXT NOT NULL,
            habitacion_id INTEGER,
            piso_id       INTEGER
        );");

    ExecuteNonQuery(c, @"
        CREATE TABLE IF NOT EXISTS Notificacion (
            id             INTEGER PRIMARY KEY AUTOINCREMENT,
            titulo         TEXT,
            mensaje        TEXT,
            tipo           TEXT,
            leida          INTEGER NOT NULL DEFAULT 0,
            fecha_creacion TEXT NOT NULL,
            fecha_lectura  TEXT,
            usuario_id     INTEGER NOT NULL
        );");

    ExecuteNonQuery(c, @"
        CREATE TABLE IF NOT EXISTS Rol (
            id                      INTEGER PRIMARY KEY AUTOINCREMENT,
            nombre                  TEXT NOT NULL,
            descripcion             TEXT,
            puede_gestionar_pisos   INTEGER NOT NULL DEFAULT 0,
            puede_ver_contratos     INTEGER NOT NULL DEFAULT 0,
            puede_gestionar_usuarios INTEGER NOT NULL DEFAULT 0
        );");

    ExecuteNonQuery(c, @"
        CREATE TABLE IF NOT EXISTS Suscripcion (
            id             INTEGER PRIMARY KEY AUTOINCREMENT,
            plan           TEXT NOT NULL,
            precio_mensual REAL NOT NULL DEFAULT 0,
            fecha_inicio   TEXT NOT NULL,
            fecha_fin      TEXT NOT NULL,
            activa         INTEGER NOT NULL DEFAULT 1,
            usuario_id     INTEGER NOT NULL
        );");


    // ─── SEED (sólo si la tabla Usuario está vacía) ───────────────
    using var chk = new SQLiteCommand("SELECT COUNT(*) FROM Usuario", c);
    var count = Convert.ToInt32(chk.ExecuteScalar());
    if (count > 0) return;

    // Seed Pisos
    ExecuteNonQuery(c, "INSERT INTO Piso(direccion,ciudad,numero_habitaciones,precio_total,descripcion,disponible) VALUES('Calle Gran Vía 42','Madrid',4,1200,'Amplio piso en el corazón de Madrid',1);");
    ExecuteNonQuery(c, "INSERT INTO Piso(direccion,ciudad,numero_habitaciones,precio_total,descripcion,disponible) VALUES('Av. Malvarrosa 15','Valencia',3,850,'Apartamento luminoso cerca de la playa',1);");
    ExecuteNonQuery(c, "INSERT INTO Piso(direccion,ciudad,numero_habitaciones,precio_total,descripcion,disponible) VALUES('Calle San Vicente 88','Alicante',3,600,'Piso ideal para estudiantes cerca de la UA',1);");


    // Seed ZonaComun
    ExecuteNonQuery(c, "INSERT INTO ZonaComun(nombre,piso_id) VALUES('Lavadora',1);");
    ExecuteNonQuery(c, "INSERT INTO ZonaComun(nombre,piso_id) VALUES('Salón',1);");

    // Seed Usuarios
    string now = DateTime.Now.ToString("o");
    ExecuteNonQuery(c, $"INSERT INTO Usuario(nombre,email,password_hash,fecha_registro,activo,rol) VALUES('Dani','dani@conviapp.com','1234','{now}',1,'Enterprise');");
    ExecuteNonQuery(c, $"INSERT INTO Usuario(nombre,email,password_hash,fecha_registro,activo,rol) VALUES('Lidia','lidia@conviapp.com','1234','{now}',1,'Profesional');");
    ExecuteNonQuery(c, $"INSERT INTO Usuario(nombre,email,password_hash,fecha_registro,activo,rol) VALUES('Marina','marina@conviapp.com','1234','{now}',1,'Basico');");
    ExecuteNonQuery(c, $"INSERT INTO Usuario(nombre,email,password_hash,fecha_registro,activo,rol) VALUES('Nazim','nazim@conviapp.com','1234','{now}',1,'Basico');");

    // Seed Tareas
    string today = DateTime.Now.ToString("o");
    ExecuteNonQuery(c, $"INSERT INTO Tarea(titulo,descripcion,estado,prioridad,fecha_creacion,creada_por_id,piso_id) VALUES('Limpiar baño principal','Limpieza completa del baño','pendiente','alta','{today}',1,1);");
    ExecuteNonQuery(c, $"INSERT INTO Tarea(titulo,descripcion,estado,prioridad,fecha_creacion,creada_por_id,piso_id) VALUES('Comprar papel higiénico','Comprar paquete de 12','pendiente','media','{today}',2,1);");
    ExecuteNonQuery(c, $"INSERT INTO Tarea(titulo,descripcion,estado,prioridad,fecha_creacion,creada_por_id,piso_id) VALUES('Sacar la basura','Contenedores de orgánico y plástico','completada','baja','{today}',1,1);");

    // Seed Gastos
    string d1 = DateTime.Now.AddDays(-5).ToString("o");
    string d2 = DateTime.Now.AddDays(-3).ToString("o");
    string d3 = DateTime.Now.AddDays(-1).ToString("o");
    ExecuteNonQuery(c, $"INSERT INTO Gasto(concepto,importe,fecha,pagado,registrado_por_id,piso_id) VALUES('Factura WiFi Abril',45.00,'{d1}',1,1,1);");
    ExecuteNonQuery(c, $"INSERT INTO Gasto(concepto,importe,fecha,pagado,registrado_por_id,piso_id) VALUES('Compra supermercado',87.50,'{d2}',0,2,1);");
    ExecuteNonQuery(c, $"INSERT INTO Gasto(concepto,importe,fecha,pagado,registrado_por_id,piso_id) VALUES('Factura electricidad',62.30,'{d3}',0,1,1);");

    // Seed Mensajes
    string m1 = DateTime.Now.AddHours(-48).ToString("o");
    string m2 = DateTime.Now.AddHours(-24).ToString("o");
    string m3 = DateTime.Now.AddHours(-2).ToString("o");
    ExecuteNonQuery(c, $"INSERT INTO Mensaje(contenido,fecha_envio,leido,emisor_id,piso_id) VALUES('¡Bienvenidos al piso! 🏠','{m1}',0,1,1);");
    ExecuteNonQuery(c, $"INSERT INTO Mensaje(contenido,fecha_envio,leido,emisor_id,piso_id) VALUES('He dejado las llaves en la entrada','{m2}',0,2,1);");
    ExecuteNonQuery(c, $"INSERT INTO Mensaje(contenido,fecha_envio,leido,emisor_id,piso_id) VALUES('¿Alguien puede comprar leche?','{m3}',0,1,1);");

    // Seed Reservas
    string r1s = DateTime.Now.AddHours(2).ToString("o");
    string r1e = DateTime.Now.AddHours(4).ToString("o");
    string r2s = DateTime.Now.AddDays(1).ToString("o");
    string r2e = DateTime.Now.AddDays(1).AddHours(3).ToString("o");
    ExecuteNonQuery(c, $"INSERT INTO Reserva(fecha_inicio,fecha_fin,estado,motivo,usuario_id,zona_comun_id) VALUES('{r1s}','{r1e}','confirmada','Lavadora',1,1);");
    ExecuteNonQuery(c, $"INSERT INTO Reserva(fecha_inicio,fecha_fin,estado,motivo,usuario_id,zona_comun_id) VALUES('{r2s}','{r2e}','confirmada','Reunión en el salón',2,2);");

    // Seed Incidencias
    string i1 = DateTime.Now.AddDays(-7).ToString("o");
    string i2 = DateTime.Now.AddDays(-3).ToString("o");
    ExecuteNonQuery(c, $"INSERT INTO Incidencia(titulo,descripcion,estado,prioridad,fecha_reporte,reportada_por_id,piso_id) VALUES('Grifo cocina gotea','El grifo de la cocina pierde agua constantemente','abierta','alta','{i1}',1,1);");
    ExecuteNonQuery(c, $"INSERT INTO Incidencia(titulo,descripcion,estado,prioridad,fecha_reporte,reportada_por_id,piso_id) VALUES('Bombilla pasillo fundida','La luz del pasillo no funciona','abierta','media','{i2}',2,1);");

    // Seed Contratos y Pagos
    string cs = new DateTime(2026,1,1).ToString("o");
    string ce = new DateTime(2026,12,31).ToString("o");
    ExecuteNonQuery(c, $"INSERT INTO Contrato(type,start_date,end_date,monthly_rent,deposit_amount,status,notes,commission_rate,property_id,user_id) VALUES('arrendamiento','{cs}','{ce}',400,800,'activo','Contrato estándar',0,1,1);");
    ExecuteNonQuery(c, $"INSERT INTO Contrato(type,start_date,end_date,monthly_rent,deposit_amount,status,notes,commission_rate,property_id,user_id) VALUES('arrendamiento','{cs}','{ce}',380,760,'activo','Sujeto a revisión semestral',0,1,2);");

    foreach (var mes in new[] { "Enero","Febrero","Marzo","Abril" })
    {
        string pd = new DateTime(2026, Array.IndexOf(new[]{"","Enero","Febrero","Marzo","Abril"}, mes), 5).ToString("o");
        ExecuteNonQuery(c, $"INSERT INTO Pago(amount,date,method,status,concept,contrato_id,user_id) VALUES(400,'{pd}','transferencia','pagado','Alquiler {mes} 2026',1,1);");
        ExecuteNonQuery(c, $"INSERT INTO Pago(amount,date,method,status,concept,contrato_id,user_id) VALUES(380,'{pd}','tarjeta','pagado','Alquiler {mes} 2026',2,2);");
    }
}

void ExecuteNonQuery(SQLiteConnection c, string sql)
{
    using var cmd = new SQLiteCommand(sql, c);
    cmd.ExecuteNonQuery();
}
