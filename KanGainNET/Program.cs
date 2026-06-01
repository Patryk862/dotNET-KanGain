using KanGainNET.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// Connection String z appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Nie znaleziono Connection String 'DefaultConnection'.");

// DB Context do kontenera DI
builder.Services.AddDbContext<SilowniaContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Konto/Logowanie";
        options.LogoutPath = "/Konto/Wyloguj";
        options.Cookie.Name = "KanGainAuth";
    });

// Obsługa kontrolerów i widoków (MVC)
builder.Services.AddControllersWithViews();

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "KanGain API",
        Version = "v1",
        Description = "Dokumentacja interfejsu API dla siłowni KanGain"
    });
});

// Serwis RFID - działa tylko w wersji lokalnej, w produkcji jest wyłączony, ponieważ nie ma fizycznego czytnika
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

// Do swaggera tylko w dev env
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "KanGain API v1");
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();