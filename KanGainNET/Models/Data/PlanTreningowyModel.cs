using System;
using System.Collections.Generic;

namespace KanGainNET.Models
{
    public class PlanTreningowy
    {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public DateTime DataStworzenia { get; set; }
        public string? Opis { get; set; }

        public int UzytkownikId { get; set; }
        public virtual Uzytkownik Uzytkownik { get; set; }

        public int TrenerId { get; set; }
        public virtual Uzytkownik Trener { get; set; }

        public virtual ICollection<Cwiczenie> Cwiczenia { get; set; } = new List<Cwiczenie>();
    }
}