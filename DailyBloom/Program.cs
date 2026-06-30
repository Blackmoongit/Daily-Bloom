using DailyBloom.Components;
using DailyBloom.Data;
using DailyBloom.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Razor components (interactive server render mode)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Database (SQLite file stored in App_Data)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")
        ?? "Data Source=App_Data/dailybloom.db"));

// App services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CurrentUserService>());
builder.Services.AddAuthorizationCore();

var app = builder.Build();

// Make sure the database & folders exist
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    Directory.CreateDirectory("App_Data");
    db.Database.EnsureCreated();
}
Directory.CreateDirectory(Path.Combine(app.Environment.WebRootPath, "uploads"));

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
