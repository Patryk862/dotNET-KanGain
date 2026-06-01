using KanGainNET.Data; 
using KanGainNET.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace KanGainNET.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SilowniaContext _context; 

        public HomeController(ILogger<HomeController> logger, SilowniaContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var karnety = await _context.TypyKarnetow.ToListAsync();
            
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

        [Authorize]
        public async Task<IActionResult> MojePlany()
        {
            var uzytkownikIdKlaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(uzytkownikIdKlaim) || !int.TryParse(uzytkownikIdKlaim, out int loggedInUserId))
            {
                return RedirectToAction("Login", "Account"); 
            }

            var mojePlany = await _context.PlanyTreningowe
                .Include(p => p.Trener)
                    .ThenInclude(t => t.Uzytkownik)
                    .ThenInclude(u => u.Profil)
                .Include(p => p.Cwiczenia)
                .Where(p => p.UzytkownikId == loggedInUserId)
                .OrderByDescending(p => p.DataStworzenia)
                .ToListAsync();

            return View(mojePlany);
        }
    }
}