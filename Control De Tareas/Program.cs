using Control_De_Tareas.Data.Entitys;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<Context>(options =>
      options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ============ CONFIGURACIÓN DE AUTENTICACIÓN Y AUTORIZACIÓN ============
// Agregar autenticación por Cookies (necesario para el sistema de roles)
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Account/Login"; // Ruta al login (crear después)
        options.AccessDeniedPath = "/Error/Error403"; // Ruta cuando no tiene permisos
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

// Configurar políticas de autorización basadas en roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SoloAdministrador", policy => policy.RequireRole("Administrador"));
    options.AddPolicy("SoloProfesor", policy => policy.RequireRole("Profesor"));
    options.AddPolicy("SoloEstudiante", policy => policy.RequireRole("Estudiante"));
    options.AddPolicy("ProfesorOAdministrador", policy => policy.RequireRole("Profesor", "Administrador"));
    options.AddPolicy("UsuarioAutenticado", policy => policy.RequireAuthenticatedUser());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ¡IMPORTANTE! La autenticación debe ir ANTES de la autorización
app.UseAuthentication();
app.UseAuthorization();

// Manejar códigos de estado HTTP (como 403, 404)
app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
