using Microsoft.EntityFrameworkCore;
using QuartzAdmin.web.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/LogOn";
    });
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=quartz_admin.db"));
builder.Services.AddScoped<IInstanceRepository, InstanceRepository>();
builder.Services.AddScoped<IConnectionRepository, ConnectionRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "multiParam",
    pattern: "{controller}/{action}/{instanceName}/{groupName}/{itemName}",
    defaults: new { controller = "Home", action = "Index" });

app.MapControllerRoute(
    name: "twoParam",
    pattern: "{controller}/{action}/{instanceName}/{itemName}",
    defaults: new { controller = "Home", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}/{id?}",
    defaults: new { controller = "Home", action = "Index" });

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }
