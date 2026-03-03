using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaphiraTerror.Infrastructure.Persistence;               // AppDbContext
using SaphiraTerror.Infrastructure.Entities;                 // ApplicationUser
using SaphiraTerror.Web.Services;
using SaphiraTerror.Web.Areas.Admin.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC + cache + HttpClient p/ API
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
{
    var baseUrl = builder.Configuration["ApiBaseUrl"]
        ?? throw new InvalidOperationException("ApiBaseUrl não configurada.");
    client.BaseAddress = new Uri(baseUrl);
});
builder.Services.ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.PropertyNameCaseInsensitive = true;
    o.SerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
});

// EF Core (mesma base)
if (!builder.Environment.IsDevelopment())
{
    var baseUrl = builder.Configuration["ApiBaseUrl"]
        ?? throw new InvalidOperationException("ApiBaseUrl não configurada.");
}
var conn = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection não configurada.");

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(conn));

// Identity + Roles (Guid) usando o MESMO user do DbContext
builder.Services
    .AddIdentityCore<ApplicationUser>(opt =>
    {
        opt.User.RequireUniqueEmail = true;
        opt.Password.RequiredLength = 6;
    })
    .AddRoles<IdentityRole<Guid>>()                 // Guid porque ApplicationUser usa Guid
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager();

// Cookies
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
})
.AddCookie(IdentityConstants.ApplicationScheme, opt =>
{
    opt.LoginPath = "/Auth/Login";
    opt.LogoutPath = "/Auth/Logout";
    opt.AccessDeniedPath = "/Auth/Denied";
    opt.SlidingExpiration = true;
});

// ?????? IMPORTANTE: policies **antes** do Build() ??????
// Policies que aceitam variações de nomes das roles (sem mexer no banco)
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("AdminOnly", policy => policy.RequireAssertion(ctx =>
        ctx.User.IsInRole("ADMIN") ||
        ctx.User.IsInRole("Admin") ||
        ctx.User.IsInRole("Administrador")
    ));

    opt.AddPolicy("ManagerOrAdmin", policy => policy.RequireAssertion(ctx =>
        ctx.User.IsInRole("ADMIN") ||
        ctx.User.IsInRole("Admin") ||
        ctx.User.IsInRole("Administrador") ||
        ctx.User.IsInRole("GERENTE") ||
        ctx.User.IsInRole("Gerente")
    ));
});
// ??????

//fase 06
// ?? Adiciona o serviço do Dashboard
builder.Services.AddScoped<DashboardService>();




// constrói o app (a partir daqui os Services ficam read-only)
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Rotas de Áreas (Admin)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

// Rota padrão
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
