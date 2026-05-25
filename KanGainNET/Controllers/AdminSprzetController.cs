using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using KanGainNET.Data;
using KanGainNET.Models;

namespace KanGainNET.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminSprzetController : Controller
    {
        private readonly SilowniaContext _context;
        public AdminSprzetController(SilowniaContext context) => _context = context;

        public async Task<IActionResult> Index(int? lokalizacjaId)
        {
            var lokalizacje = await _context.Lokalizacje.ToListAsync();
            var wybraneId = lokalizacjaId ?? lokalizacje.FirstOrDefault()?.Id;

            var sprzet = await _context.Sprzet
                .Include(s => s.Lokalizacja)
                .Where(s => s.LokalizacjaId == wybraneId)
                .ToListAsync();

            ViewBag.Lokalizacje = lokalizacje;
            ViewBag.WybraneId = wybraneId;

            return View("~/Views/Admin/Maszyny.cshtml", sprzet);
        }

        [HttpPost]
        public async Task<IActionResult> ZmienStan(int id, string stan)
        {
            var s = await _context.Sprzet.FindAsync(id);
            if (s != null)
            {
                s.StanTechniczny = stan;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { lokalizacjaId = s?.LokalizacjaId });
        }
    }
}