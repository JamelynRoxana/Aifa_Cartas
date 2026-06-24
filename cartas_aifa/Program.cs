using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using cartas_aifa.Data;

var builder = WebApplication.CreateBuilder(args);

// Usar cultura invariante para binding de decimales (punto como separador)
System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;

// EF Core - SQL Server (conexión externa)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("Falta configurar Jwt:Secret en appsettings.json");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = "CartasAifaApi",
            ValidAudience = "CartasAifaApiUsers",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

// MVC + API Controllers
builder.Services.AddControllersWithViews(options =>
{
    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => "Este campo es obligatorio.");
    options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, _) => $"El valor '{x}' no es válido.");
    options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() => "Este campo es obligatorio.");
    options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(x => $"El valor '{x}' no es válido.");
});

var app = builder.Build();

// Puerto para Render/Docker
var port = Environment.GetEnvironmentVariable("PORT") ?? "5031";
app.Urls.Add($"http://0.0.0.0:{port}");

// Aplicar columnas pendientes si no existen
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.ExecuteSqlRaw(@"
            IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TramitesAcademicos') AND name = 'FechaInicioPeriodo')
            BEGIN
                ALTER TABLE TramitesAcademicos ADD FechaInicioPeriodo datetime2 NULL;
            END
            IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TramitesAcademicos') AND name = 'FechaFinPeriodo')
            BEGIN
                ALTER TABLE TramitesAcademicos ADD FechaFinPeriodo datetime2 NULL;
            END
            IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Estudiantes') AND name = 'PlanEstudios')
            BEGIN
                ALTER TABLE Estudiantes ADD PlanEstudios NVARCHAR(50) NULL;
            END
            IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Estudiantes') AND name = 'PeriodoActual')
            BEGIN
                ALTER TABLE Estudiantes ADD PeriodoActual INT NULL;
            END
            IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('TramitesAcademicos') AND name = 'TotalHoras')
            BEGIN
                ALTER TABLE TramitesAcademicos ADD TotalHoras INT NULL;
            END
            IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Usuarios') AND name = 'AvatarUrl')
            BEGIN
                ALTER TABLE Usuarios ADD AvatarUrl NVARCHAR(500) NULL;
            END
        ");
    }
    catch { /* Si falla, las columnas/tablas ya existen o la DB no está disponible */ }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Rutas MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Rutas API
app.MapControllers();

app.Run();
