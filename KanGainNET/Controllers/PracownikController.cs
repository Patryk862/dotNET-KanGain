using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KanGainNET.Controllers
{
    // Upewnij się, że nazwa w bazie dla RolaId = 3 to "Pracownik" lub zmień ten string (np. na "Trener")
    [Authorize(Roles = "Trener")] 
    public class PracownikController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}