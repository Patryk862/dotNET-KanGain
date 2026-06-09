using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KanGainNET.Data;
using KanGainNET.Models;

namespace KanGainNET.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersApiController : ControllerBase
    {
        private readonly SilowniaContext _context;

        public UsersApiController(SilowniaContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Uzytkownik>>> GetUzytkownicy()
        {
            return await _context.Uzytkownicy.Include(u => u.Rola).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Uzytkownik>> GetUzytkownik(int id)
        {
            var user = await _context.Uzytkownicy.Include(u => u.Rola).FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            return user;
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class TrainersApiController : ControllerBase
    {
        private readonly SilowniaContext _context;

        public TrainersApiController(SilowniaContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Uzytkownik>>> GetTrainers()
        {
            var trainers = await _context.Uzytkownicy
                .AsNoTracking()
                .Include(u => u.Profil) 
                .Where(u => u.RolaId == 3)
                .ToListAsync();

            if (!trainers.Any())
            {
                return NotFound("Obecnie brak dostępnych trenerów.");
            }

            return Ok(trainers);
        }
    }
}