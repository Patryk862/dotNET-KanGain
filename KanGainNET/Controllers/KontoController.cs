using Microsoft.AspNetCore.Mvc;
using KanGainNET.Data;
using KanGainNET.Models;
using KanGainNET.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace KanGainNET.Controllers
{
    public class KontoController : Controller
    {
        private readonly SilowniaContext _context;

        public KontoController(SilowniaContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Rejestracja() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rejestracja(RejestracjaViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _context.Uzytkownicy.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Ten adres email jest już zarejestrowany.");
                return View(model);
            }

            var nowyUzytkownik = new Uzytkownik
            {
                Email = model.Email,
                Haslo = HashujHaslo(model.Haslo),
                DataRejestracji = DateTime.Now,
                RolaId = 3, // Klient
                Profil = new ProfilUzytkownika
                {
                    Imie = model.Imie,
                    Nazwisko = model.Nazwisko,
                    Telefon = model.Telefon
                }
            };

            _context.Uzytkownicy.Add(nowyUzytkownik);
            await _context.SaveChangesAsync();

            // Po rejestracji przekierowujemy do logowania
            TempData["Sukces"] = "Konto zostało utworzone. Możesz się zalogować.";
            return RedirectToAction("Logowanie"); 
        }

        // --- LOGOWANIE ---
        [HttpGet]
        public IActionResult Logowanie()
        {
            // Jeśli użytkownik jest już zalogowany, odeślij go na stronę główną
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logowanie(LogowanieViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            string zhashowaneHaslo = HashujHaslo(model.Haslo);

            // Szukamy użytkownika w bazie i od razu "wyciągamy" jego Profil i Rolę
            var uzytkownik = await _context.Uzytkownicy
                .Include(u => u.Profil)
                .Include(u => u.Rola)
                .FirstOrDefaultAsync(u => u.Email == model.Email && u.Haslo == zhashowaneHaslo);

            if (uzytkownik == null)
            {
                ModelState.AddModelError(string.Empty, "Nieprawidłowy email lub hasło.");
                return View(model);
            }

            // Tworzenie "dowodu tożsamości" (Claims) dla ciasteczka
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, uzytkownik.Id.ToString()),
                new Claim(ClaimTypes.Email, uzytkownik.Email),
                new Claim(ClaimTypes.Name, uzytkownik.Profil.Imie), // Wyświetlimy to w Navbarze
                new Claim(ClaimTypes.Role, uzytkownik.Rola.Nazwa)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Zapisanie ciasteczka logowania w przeglądarce
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        // --- WYLOGOWANIE ---
        [HttpPost]
        public async Task<IActionResult> Wyloguj()
        {
            // Usunięcie ciasteczka
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // --- HASHOWANIE ---
        private string HashujHaslo(string haslo)
        {
            if (string.IsNullOrEmpty(haslo)) return string.Empty;
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(haslo));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++) builder.Append(bytes[i].ToString("x2"));
                return builder.ToString();
            }
        }
    }
}