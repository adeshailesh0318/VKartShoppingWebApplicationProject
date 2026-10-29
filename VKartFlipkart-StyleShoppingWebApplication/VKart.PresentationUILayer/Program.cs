using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VKart.DataAccessLayer.Context;
using VKart.DataAccessLayer.Seed;
using VKart.ModelsLayer.Entities;
using VKart.RepositoriesLayer;
using VKart.ServicesLayer;

var builder = WebApplication.CreateBuilder(args);

// ===== 1. DbContext =====
builder.Services.AddDbContext<VKartDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===== 2. Identity with COOKIE authentication (MVC) =====
builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>(opts =>
    {
        opts.Password.RequireDigit = true;
        opts.Password.RequireLowercase = true;
        opts.Password.RequireUppercase = true;
        opts.Password.RequireNonAlphanumeric = true;
        opts.Password.RequiredLength = 6;
        opts.User.RequireUniqueEmail = true;

        opts.SignIn.RequireConfirmedEmail = false;
    })
    .AddEntityFrameworkStores<VKartDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.LoginPath = "/Account/Login";
    opts.LogoutPath = "/Account/Logout";
    opts.AccessDeniedPath = "/Account/AccessDenied";
    opts.Cookie.Name = "VKart.Auth";
    opts.ExpireTimeSpan = TimeSpan.FromHours(2);
    opts.SlidingExpiration = true;
});

// ===== 3. Repositories + Services =====
builder.Services.AddRepositories();
builder.Services.AddServices(builder.Configuration);

// ===== 4. MVC + HTTP client + Session =====
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddSession();

var app = builder.Build();

// ===== 5. Auto-migrate + Seed =====
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<VKartDbContext>();
    await db.Database.MigrateAsync();
}
await DbSeeder.SeedAsync(app.Services);

// ===== 6. Pipeline =====
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// ===== 7. ROUTING (Order matters!) =====

// 1. Area Route (PEHLE aana chahiye)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// 2. Default Route (BAAD mein aana chahiye)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();