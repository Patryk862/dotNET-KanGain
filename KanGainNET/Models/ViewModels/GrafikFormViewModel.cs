using System;
using System.Collections.Generic;
using KanGainNET.Models;

namespace KanGainNET.Models.ViewModels
{
    public class GrafikFormViewModel
    {
        public int? Id { get; set; }

        public DateTime DataStart { get; set; }
        public DateTime DataKoniec { get; set; }
        public int SalaId { get; set; }

        public int? ZajeciaGrupoweId { get; set; }
        public int? PracownikId { get; set; }

        public List<Sala>? DostepneSale { get; set; }
        public List<ZajeciaGrupowe>? DostepneZajecia { get; set; }

        public List<Uzytkownik>? DostepniPracownicy { get; set; }

        public List<string> DostepneGodziny { get; set; } = new List<string>();
        public bool CzyTylkoOdczyt { get; set; }
    }
}