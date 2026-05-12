using System;
using System.Collections.Generic;
using KanGainNET.Models;

namespace KanGainNET.Models.ViewModels
{
    public class GrafikViewModel
    {
        // --- WIDOK GŁÓWNY (INDEX) ---
        public DateTime Data { get; set; }
        public List<Sala>? Sale { get; set; }
        public List<Grafik>? Zajecia { get; set; }
        public List<DateTime>? GodzinySiatki { get; set; }
        public List<Lokalizacja>? Lokalizacje { get; set; }
        public int? WybranaLokalizacjaId { get; set; }

        // --- FORMULARZ (DODAWANIE / EDYCJA / PODGLĄD) ---
        public int? Id { get; set; }
        public DateTime DataStart { get; set; }
        public DateTime DataKoniec { get; set; }
        public int SalaId { get; set; }
        public int? ZajeciaGrupoweId { get; set; }
        public int? PracownikId { get; set; }

        public List<Sala>? DostepneSale { get; set; }
        public List<ZajeciaGrupowe>? DostepneZajecia { get; set; }
        public List<Pracownik>? DostepniPracownicy { get; set; }
        public List<string>? ListaGodzinDropdown { get; set; }
        public bool CzyTylkoOdczyt { get; set; }
    }
}