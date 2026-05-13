using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KanGainNET.Controllers
{
    // Tylko użytkownicy, którzy w tabeli Role mają nazwę "Admin" (RolaId = 1) wejdą tutaj
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}