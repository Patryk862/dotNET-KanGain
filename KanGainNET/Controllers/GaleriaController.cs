using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace KanGainNET.Controllers
{
    public class GaleriaController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly string _folderPath;

        public GaleriaController(IWebHostEnvironment env)
        {
            _env = env;
            // Wyciągamy ścieżkę do jednego miejsca, żeby nie powtarzać kodu
            _folderPath = Path.Combine(_env.ContentRootPath, "ZdjeciaGalerii");
        }

        public IActionResult Index()
        {
            var listaZdjec = new List<string>();

            if (Directory.Exists(_folderPath))
            {
                var rozszerzenia = new[] { "*.jpg", "*.jpeg", "*.png", "*.webp" };
                var pliki = rozszerzenia.SelectMany(r => Directory.GetFiles(_folderPath, r)).ToList();

                foreach (var plik in pliki)
                {
                    var nazwaPliku = Path.GetFileName(plik);
                    // Przekazujemy do widoku url akcji, ale sam widok będzie musiał znać też nazwę pliku do usunięcia
                    listaZdjec.Add(nazwaPliku);
                }
            }

            return View(listaZdjec);
        }

        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
        public IActionResult Zdjecie(string nazwa)
        {
            if (string.IsNullOrEmpty(nazwa)) return NotFound();

            nazwa = Path.GetFileName(nazwa);
            var filePath = Path.Combine(_folderPath, nazwa);

            if (!System.IO.File.Exists(filePath)) return NotFound();

            var ext = Path.GetExtension(nazwa).ToLowerInvariant();
            string mimeType = ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };

            return PhysicalFile(filePath, mimeType);
        }

        // Akcje dla admina

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Zapisz(IFormFile plikObrazu)
        {
            if (plikObrazu != null && plikObrazu.Length > 0)
            {
                // Walidacja rozszerzenia pliku
                var ext = Path.GetExtension(plikObrazu.FileName).ToLowerInvariant();
                var dozwoloneRozszerzenia = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                if (!dozwoloneRozszerzenia.Contains(ext))
                {
                    TempData["Error"] = "Niedozwolony format pliku! Wybierz JPG, PNG lub WEBP.";
                    return RedirectToAction("Index");
                }

                // Upewniamy się, że folder istnieje
                if (!Directory.Exists(_folderPath))
                {
                    Directory.CreateDirectory(_folderPath);
                }

                // Zabezpieczenie nazwy pliku i unikalność (zapobiega nadpisywaniu)
                var oryginalnaNazwa = Path.GetFileName(plikObrazu.FileName);
                var unikalnaNazwa = $"{Path.GetFileNameWithoutExtension(oryginalnaNazwa)}_{System.Guid.NewGuid()}{ext}";
                var pelnaSciezka = Path.Combine(_folderPath, unikalnaNazwa);

                using (var stream = new FileStream(pelnaSciezka, FileMode.Create))
                {
                    await plikObrazu.CopyToAsync(stream);
                }

                TempData["Success"] = "Zdjęcie zostało pomyślnie dodane do galerii!";
            }
            else
            {
                TempData["Error"] = "Nie wybrano żadnego pliku lub plik jest pusty.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Usun(string nazwa)
        {
            if (string.IsNullOrEmpty(nazwa)) return NotFound();

            nazwa = Path.GetFileName(nazwa);
            var filePath = Path.Combine(_folderPath, nazwa);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                TempData["Success"] = "Zdjęcie zostało usunięte.";
            }
            else
            {
                TempData["Error"] = "Nie znaleziono pliku do usunięcia.";
            }

            return RedirectToAction("Index");
        }
    }
}