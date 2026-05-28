using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace KanGainNET.Controllers
{
    public class GaleriaController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public GaleriaController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public IActionResult Index()
        {
            // Zmieniamy ścieżkę na nowy, zablokowany przed przeglądarką folder
            var folderPath = Path.Combine(_env.ContentRootPath, "ZdjeciaGalerii");
            var listaZdjec = new List<string>();

            if (Directory.Exists(folderPath))
            {
                var rozszerzenia = new[] { "*.jpg", "*.jpeg", "*.png", "*.webp" };
                var pliki = rozszerzenia.SelectMany(r => Directory.GetFiles(folderPath, r)).ToList();

                foreach (var plik in pliki)
                {
                    var nazwaPliku = Path.GetFileName(plik);
                    
                    // Zamiast podawać bezpośredni link do pliku, generujemy link do naszej bezpiecznej akcji
                    listaZdjec.Add(Url.Action("Zdjecie", "Galeria", new { nazwa = nazwaPliku }));
                }
            }

            return View(listaZdjec);
        }

        // Ta akcja działa jak bramkarz - serwuje obrazek bezpośrednio z kodu
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)] // Cache na 24h, żeby działało szybciej
        public IActionResult Zdjecie(string nazwa)
        {
            if (string.IsNullOrEmpty(nazwa)) return NotFound();

            // ZABEZPIECZENIE: Upewniamy się, że to tylko nazwa pliku (blokuje ataki typu Path Traversal)
            nazwa = Path.GetFileName(nazwa); 
            
            var filePath = Path.Combine(_env.ContentRootPath, "ZdjeciaGalerii", nazwa);

            if (!System.IO.File.Exists(filePath)) return NotFound();

            // Ustalanie poprawnego formatu MIME dla przeglądarki
            var ext = Path.GetExtension(nazwa).ToLowerInvariant();
            string mimeType = ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };

            // Zwracamy fizyczny plik z ukrytego folderu
            return PhysicalFile(filePath, mimeType);
        }
    }
}