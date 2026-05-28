using Microsoft.AspNetCore.Mvc.Rendering;
using KanGainNET.Models;

namespace KanGainNET.Models.ViewModels
{
    public class DodajPlanViewModel
    {
        public int? Id { get; set; }
        public string Nazwa { get; set; }
        public int WybranyUzytkownikId { get; set; }
        public int[] WybraneCwiczeniaIds { get; set; } = new int[0];
        public List<SelectListItem> Klubowicze { get; set; } = new List<SelectListItem>();
        public List<Cwiczenie> WszystkieCwiczenia { get; set; } = new List<Cwiczenie>();
    }
}