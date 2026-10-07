using FilmLibrary.Data;
using FilmLibrary.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Base de données
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("Default")));

// Identity
builder.Services
    .AddDefaultIdentity<IdentityUser>(
        options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// MVC + Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

// Injection de dépendances
builder.Services.AddScoped<IFilmService, FilmService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

var app = builder.Build();

// Initialisation de la base
using (var scope = app.Services.CreateScope())
{
    await DbInitializer.InitializeAsync(scope.ServiceProvider);
}

// Configuration HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

// Route MVC par défaut
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Films}/{action=Index}/{id?}");

// Pages Identity
app.MapRazorPages();

app.Run();
