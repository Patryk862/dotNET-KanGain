using KanGainNET.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using KanGainNET.Data; // Dodane dla SilowniaContext
using Microsoft.EntityFrameworkCore; // Dodane dla ToListAsync()

namespace KanGainNET.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SilowniaContext _context; // Dodane pole na bazę danych

        // Wstrzykujemy oba serwisy przez konstruktor
        public HomeController(ILogger<HomeController> logger, SilowniaContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Zmieniamy na async Task, żeby asynchronicznie pobrać dane z bazy
        public async Task<IActionResult> Index()
        {
            // Pobieramy listę karnetów z bazy danych
            var karnety = await _context.TypyKarnetow.ToListAsync();
            
            // Przekazujemy listę do widoku (Index.cshtml)
            return View(karnety);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> LokalizacjeMapa()
        {
            var kluby = await _context.Lokalizacje.ToListAsync();
            return View(kluby);
        }
    }
}