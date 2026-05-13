using Microsoft.EntityFrameworkCore;
using KanGainNET.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// 1. Pobierz Connection String z appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Nie znaleziono Connection String 'DefaultConnection'.");

// 2. Dodaj Context do kontenera DI
builder.Services.AddDbContext<SilowniaContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Konto/Logowanie"; // Gdzie odesłać niezalogowanego
        options.LogoutPath = "/Konto/Wyloguj";
        options.Cookie.Name = "KanGainAuth";
    });

// Dodaj obsługę kontrolerów i widoków (MVC)
builder.Services.AddControllersWithViews();

//builder.Services.AddSingleton<RFIDReaderService>();
Stripe.StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

var app = builder.Build();

//app.Services.GetRequiredService<RFIDReaderService>();

// Konfiguracja potoku HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();